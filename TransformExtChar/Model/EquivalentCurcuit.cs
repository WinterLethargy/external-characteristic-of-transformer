using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TransformExtChar.Model
{
    public class EquivalentCurcuit
    {
        #region Параметры схемы замещения

        public Complex Zm { get; set; }
        public Complex Z1 { get; set; }
        public Complex Z2_Сorrected { get; set; }
        public double K { get; set; }

        #endregion

        #region constructors
        public EquivalentCurcuit() { } // из-за определения конструктора с параметрами конструктор без параметров нужно создать явно
        public EquivalentCurcuit(TransformerDatasheetSpecifications specifications)
        {
            if (specifications.TryGetEquivalentCurcuitParameters(out var parameters)) // иначе все параметры будут инициализированы нулями
            {
                Zm = parameters.Zm;
                Z1 = parameters.Z1;
                Z2_Сorrected = parameters.Z2_Сorrected;
                K = parameters.K;
            }
        }
        #endregion

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

            if (fi2_rad > Math.PI / 2 || fi2_rad < -Math.PI / 2) return new List<VCData>(); // угол нагрузки лежит в пределах +-PI/2

            if (U1 == 0) U1 = 220; //как будто включили в розетку

            if (I2_correctedStart > I2_correctedEnd)    // если перепутаны начало и конец расчетного интервала, то поменять их местами
            {
                double temp = I2_correctedEnd;
                I2_correctedEnd = I2_correctedStart;
                I2_correctedStart = temp;
            }

            if (I2_correctedEnd == 0) return ComputeExternalCharacteristicWhile(fi2_rad, 0, U1, I2_step, I2_corectCurrent => true); 
                                                                                                  // если начало и конец расчетного интервала совпадают
                                                                                                  // на 0, то вернуть характиристику от холостого хода
                                                                                                  // до короткого замыкания
            return ComputeExternalCharacteristicWhile(fi2_rad, I2_correctedStart, U1, I2_step, I2_corectCurrent => I2_corectCurrent < I2_correctedEnd);
        }

        public List<VCData> GetFullExternalCharacteristic(double fi2_rad = 0, double U1 = 0, double I2_step = 0.01)
        {
            if (U1 < 0 || I2_step <= 0) return new List<VCData>();

            if (fi2_rad > Math.PI / 2 || fi2_rad < -Math.PI / 2) return new List<VCData>();

            if (U1 == 0) U1 = 220; //как будто включили в розетку

            return ComputeExternalCharacteristicWhile(fi2_rad, 0, U1, I2_step, I2_corectCurrent => true);
        }

        private List<VCData> ComputeExternalCharacteristicWhile(double fi2_rad, double I2_correctedStart,
                                                      double U1, double I2_step, Predicate<double> predicate)
        {
            var ExternalCharacteristic = new List<VCData>();

            if (Zm == 0) return ExternalCharacteristic;         // трансформатор не передает энергию
                                                                // не получится посчитать угол напряжения на нагрузке
            Complex Z1Zm_sum = Z1 + Zm;
            
            if (Z1Zm_sum == 0) return ExternalCharacteristic;   // находится в знаменателе нескольких выражений

            Complex Za = -Zm / Z1Zm_sum;
            Complex Zb = -Z2_Сorrected - Zm + Zm * Zm / Z1Zm_sum;

            Complex U1_complex = Complex.FromPolarCoordinates(U1, -Za.Phase);
            Complex ZaU1_mult = Za * U1_complex;

            double currentI2_corrected = I2_correctedStart;

            var ExternalCharacteristicReverseBrunch = new LinkedList<(double magnitude, double psiI2)>(); // здесь будут точки тех режимов,
                                                                                                          // когда один ток реализуется при разных напряжениях
            double b_less_fi2 = Zb.Phase - fi2_rad;
            bool isPositive = b_less_fi2 > 0 ? true : false;    // лежат треугольники в верхней или нижней полуплоскости
                                                                // нужно для проверки сопряженных тупых углов напряжения нагрузки

            while (predicate(currentI2_corrected))
            {
                double psiU2 = Math.Asin(currentI2_corrected * Zb.Magnitude * Math.Sin(b_less_fi2) / ZaU1_mult.Magnitude);
                if (double.IsNaN(psiU2)) break;

                double psiI2 = psiU2 - fi2_rad;
                conjugateAngleCheсk(isPositive, psiU2);

                Complex I2_correctedComplex = Complex.FromPolarCoordinates(currentI2_corrected, psiI2);

                Complex U2_CorrectedComplex = ZaU1_mult + Zb * I2_correctedComplex;
                Complex Z_loadCorrected = U2_CorrectedComplex / I2_correctedComplex;

                if (Math.Abs(Z_loadCorrected.Phase - fi2_rad) > 1E-10) break; // алгоритм всё хорошо считает от холостого хода
                                                                              // до короткого замыкания, но потом меняет фазу на 180 градусов
                                                                              // и продолжает считать точки с сопротивлением нагрузки fi2 + 180
                                                                              // до тех пор пока не берётся арксинус числа большего 1 ранее в коде
                AddPoint(ref I2_correctedComplex, ref U2_CorrectedComplex, ref Z_loadCorrected);

                currentI2_corrected += I2_step;
            }

            foreach (var I2_corrected in ExternalCharacteristicReverseBrunch) // теперь считать точки обратной ветви. если эти точки считать сразу, то
            {                                                                 // не получится начертить красивый график одной линией
                Complex I2_correctedComplex = Complex.FromPolarCoordinates(I2_corrected.magnitude, I2_corrected.psiI2);

                Complex U2_CorrectedComplex = ZaU1_mult + Zb * I2_correctedComplex;
                Complex Z_loadCorrected = U2_CorrectedComplex / I2_correctedComplex;

                AddPoint(ref I2_correctedComplex, ref U2_CorrectedComplex, ref Z_loadCorrected);
            }

            return ExternalCharacteristic;

            void conjugateAngleCheсk(bool isPositive, double psiU2)
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

