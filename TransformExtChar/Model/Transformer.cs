using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TransformExtChar.Model
{
    public class Transformer
    {
        #region Parameters
        private Complex _Zm;

        public Complex Zm
        {
            get { return _Zm; }
            set
            {
                if (_Zm == value) return;
                _dataSheet = null;
                _Zm = value;
            }
        }

        private Complex _Z1;

        public Complex Z1
        {
            get { return _Z1; }
            set
            {
                if (_Z1 == value) return;
                _dataSheet = null;
                _Z1 = value;
            }
        }

        private Complex _Z2_Сorrected;

        public Complex Z2_Сorrected
        {
            get { return _Z2_Сorrected; }
            set
            {
                if (_Z2_Сorrected == value) return;
                _dataSheet = null;
                _Z2_Сorrected = value;
            }
        }

        private double _K;

        public double K
        {
            get { return _K; }
            set
            {
                if (_K == value) return;
                _dataSheet = null;
                _K = value;
            }
        }

        private TransformerDatasheetSpecifications _dataSheet;

        #endregion

        #region constructors
    public Transformer()
        {

        }
        public Transformer(TransformerDatasheetSpecifications specifications)
        {
            if (specifications.TryGetTransformerParameters(out var parameters)) // иначе все параметры будут инициализированы нулями
            {
                _dataSheet = specifications;
                _Zm = parameters.Zm;
                _Z1 = parameters.Z1;
                _Z2_Сorrected = parameters.Z2_Сorrected;
                _K = parameters.K;
            }
        }
        #endregion

        public bool TryGetDataSheet(out TransformerDatasheetSpecifications specifications)
        {
            specifications = _dataSheet;
            return _dataSheet == null ? false : true;
        }

        public Task<List<VCData>> GetExternalCharacteristicAsync(double fi2_rad = 0, double I2_correctedStart = 0, double I2_correctedEnd = 0,
                                                      double U1 = 0, double I2_step = 0.01)
        {
            return Task.Run(() => GetExternalCharacteristic(fi2_rad, I2_correctedStart, I2_correctedEnd, U1, I2_step));
        }

        public Task<List<VCData>> GetFullExternalCharacteristicAsync(double fi2_rad = 0, double U1 = 0, double I2_step = 0.1)
        {
            return Task.Run(() => GetFullExternalCharacteristic(fi2_rad, U1, I2_step));
        }

        public List<VCData> GetExternalCharacteristic(double fi2_rad = 0, double I2_correctedStart = 0, double I2_correctedEnd = 0, 
                                                      double U1 = 0, double I2_step = 0.01)
        {
            if (I2_correctedStart < 0 || I2_correctedEnd < 0 || U1 < 0 || I2_step <= 0) return new List<VCData>();

            if (fi2_rad > Math.PI / 2 || fi2_rad < -Math.PI / 2) return new List<VCData>();

            bool isDataSheetExists = TryGetDataSheet(out TransformerDatasheetSpecifications specifications);

            if (U1 == 0) U1 = isDataSheetExists ? specifications.U1_Rated : 220; //как будто включили в розетку

            if (I2_correctedStart > I2_correctedEnd)
            {
                double temp = I2_correctedEnd;
                I2_correctedEnd = I2_correctedStart;
                I2_correctedStart = temp;
            }

            if (I2_correctedEnd == 0) I2_correctedEnd = isDataSheetExists ? specifications.I1_Rated * K * 1.1 : 0;

            if (I2_correctedEnd == 0) return GetFullExternalCharacteristic(fi2_rad, U1, I2_step);

            return ComputeExternalCharacteristicWhile(fi2_rad, I2_correctedStart, U1, I2_step, I2_corectCurrent => I2_corectCurrent < I2_correctedEnd);
        }

        public List<VCData> GetFullExternalCharacteristic(double fi2_rad = 0, double U1 = 0, double I2_step = 0.01)
        {
            if (U1 < 0 || I2_step <= 0) return new List<VCData>();

            if (fi2_rad > Math.PI / 2 || fi2_rad < -Math.PI / 2) return new List<VCData>();

            bool isDataSheetExists = TryGetDataSheet(out var specifications);

            if (U1 == 0) U1 = isDataSheetExists ? specifications.U1_Rated : 220; //как будто включили в розетку

            return ComputeExternalCharacteristicWhile(fi2_rad, 0, U1, I2_step, I2_corectCurrent => true);
        }

        private List<VCData> ComputeExternalCharacteristicWhile(double fi2_rad, double I2_correctedStart,
                                                      double U1, double I2_step, Predicate<double> predicate)
        {
            if (Zm == 0) return new List<VCData>();

            Complex Z1Zm_sum = Z1 + Zm;
            
            if (Z1Zm_sum == 0) return new List<VCData>();

            Complex Za = -Zm / Z1Zm_sum;
            Complex Zb = -Z2_Сorrected - Zm + Zm * Zm / Z1Zm_sum;

            Complex U1_complex = Complex.FromPolarCoordinates(U1, -Za.Phase);
            Complex ZaU1_mult = Za * U1_complex;

            double currentI2_corrected = I2_correctedStart;

            var ExternalCharacteristic = new List<VCData>();
            var ExternalCharacteristicReverseBrunch = new LinkedList<(double magnitude, double psiI2)>();

            double b_less_fi2 = Zb.Phase - fi2_rad;
            bool isPositive = b_less_fi2 > 0 ? true : false;


            while (predicate(currentI2_corrected))
            {
                double psiU2 = Math.Asin(currentI2_corrected * Zb.Magnitude * Math.Sin(b_less_fi2) / ZaU1_mult.Magnitude);
                if (double.IsNaN(psiU2)) break;

                double psiI2 = psiU2 - fi2_rad;
                conjugateAngleChek(isPositive, psiU2);

                Complex I2_correctedComplex = Complex.FromPolarCoordinates(currentI2_corrected, psiI2);

                Complex U2_CorrectedComplex = ZaU1_mult + Zb * I2_correctedComplex;
                Complex Z_loadCorrected = U2_CorrectedComplex / I2_correctedComplex;

                if (Math.Abs(Z_loadCorrected.Phase - fi2_rad) > 1E-10) break; // фаза изменилась на 180 градусов (не пойму почему так происходит)



                AddPoint(ref I2_correctedComplex, ref U2_CorrectedComplex, ref Z_loadCorrected);

                currentI2_corrected += I2_step;
            }

            foreach (var I2_corrected in ExternalCharacteristicReverseBrunch)
            {
                Complex I2_correctedComplex = Complex.FromPolarCoordinates(I2_corrected.magnitude, I2_corrected.psiI2);

                Complex U2_CorrectedComplex = ZaU1_mult + Zb * I2_correctedComplex;
                Complex Z_loadCorrected = U2_CorrectedComplex / I2_correctedComplex;

                AddPoint(ref I2_correctedComplex, ref U2_CorrectedComplex, ref Z_loadCorrected);
            }

            return ExternalCharacteristic;

            void conjugateAngleChek(bool isPositive, double psiU2)
            {
                double one = isPositive ? 1 : -1;

                double conjugateAngle = one * Math.PI - psiU2; // сопряженный тупой угол
                double psiI2 = conjugateAngle - fi2_rad;
                double secondAngle = Math.PI - (one * (Zb.Phase + psiI2));

                if (secondAngle < Math.PI / 2 && secondAngle > 0) ExternalCharacteristicReverseBrunch.AddFirst((currentI2_corrected, psiI2));
            }

            void AddPoint(ref Complex I2_corrected, ref Complex U2_corrected, ref Complex Z_loadCorrected)
            {
                VCData data = new VCData
                {
                    Current = I2_corrected.Magnitude,
                    Voltage = U2_corrected.Magnitude,
                    Z_load = Z_loadCorrected
                };

                ExternalCharacteristic.Add(data);
            }
        }
    }
}

