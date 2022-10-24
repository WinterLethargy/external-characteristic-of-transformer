using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using TransformExtChar.Infrastructure;

namespace TransformExtChar.Model
{
    public class TransformerDatasheet : ITransformerDatasheet
    {
        #region Property
        public ITransformerConfig TransformerConfig { get; set; }
        public double U1r { get; set; }
        public double U2r { get; set; }
        public double I1r { get; set; }
        public double I0 { get; set; }
        public double I0_Percent
        {
            get
            {
                var result = I0 / I1r * 100;
                if (double.IsNaN(result))
                    return 0;
                return result;
            }
            set => I0 = value / 100 * I1r;
        }
        public double P0 { get; set; }
        public double U1sc { get; set; }
        public double U1sc_Percent
        {
            get
            {
                var result = U1sc / U1r * 100;
                if (double.IsNaN(result))
                    return 0;
                return result;
            }
            set => U1sc = value / 100 * U1r;
        }
        public double Psc { get; set; }
        #endregion

        #region словари и методы, возвращающие множители для сопротивлений или перевода в фазные величины
        private Dictionary<TransformerTypeEnum, Func<(double Z0_Gain, double R0_Gain, double Z_ShortCircuit_Gain, double R_ShortCircuit_Gain)>> TransformerTypeRecalculatedCoefficientDictionary;

        private Dictionary<StarOrTriangleEnum, Func<(double Z0_Gain, double R0_Gain, double Z_ShortCircuit_Gain, double R_ShortCircuit_Gain)>> StarOrTriangleRecalculatedCoefficientDictionary;
        private (double Z0_Gain, double R0_Gain, double Z_ShortCircuit_Gain, double R_ShortCircuit_Gain) GetRecalculatedCoefficientNone() => (1, 1, 1, 1);
        private (double Z0_Gain, double R0_Gain, double Z_ShortCircuit_Gain, double R_ShortCircuit_Gain) GetRecalculatedCoefficientOnePhase() => (1, 1, 1, 1);
        private (double Z0_Gain, double R0_Gain, double Z_ShortCircuit_Gain, double R_ShortCircuit_Gain) GetRecalculatedCoefficientThreePhase() => StarOrTriangleRecalculatedCoefficientDictionary[TransformerConfig.FirstWinding].Invoke();
        private (double Z0_Gain, double R0_Gain, double Z_ShortCircuit_Gain, double R_ShortCircuit_Gain) GetRecalculatedCoefficientThreePhaseFirstWindingStar() => (1 / Math.Sqrt(3), 1.0 / 3.0, 1 / Math.Sqrt(3), 1.0 / 3.0);
        private (double Z0_Gain, double R0_Gain, double Z_ShortCircuit_Gain, double R_ShortCircuit_Gain) GetRecalculatedCoefficientThreePhaseFirstingTriangle() => (Math.Sqrt(3), 1, Math.Sqrt(3), 1);
        private static Dictionary<StarOrTriangleEnum, double> ToPhaseVoltageGain { get; }
        #endregion
        public TransformerDatasheet() : this(new TransformerConfig()) { }
        static TransformerDatasheet() 
        {
            ToPhaseVoltageGain = new Dictionary<StarOrTriangleEnum, double>
            {
                [StarOrTriangleEnum.None] = 1,
                [StarOrTriangleEnum.Triangle] = 1,
                [StarOrTriangleEnum.Star] = 1 / Math.Sqrt(3)
            };
        }
        public TransformerDatasheet(ITransformerConfig transformerConfig)
        {
            TransformerConfig = transformerConfig;
            TransformerTypeRecalculatedCoefficientDictionary = new Dictionary<TransformerTypeEnum, Func<(double Z0_Gain, double R0_Gain, double Z_ShortCircuit_Gain, double R_ShortCircuit_Gain)>>()
            {
                { TransformerTypeEnum.None, GetRecalculatedCoefficientNone },
                { TransformerTypeEnum.OnePhase, GetRecalculatedCoefficientOnePhase },
                { TransformerTypeEnum.ThreePhase,  GetRecalculatedCoefficientThreePhase }
            };

            StarOrTriangleRecalculatedCoefficientDictionary = new Dictionary<StarOrTriangleEnum, Func<(double Z0_Gain, double R0_Gain, double Z_ShortCircuit_Gain, double R_ShortCircuit_Gain)>>()
            {
                {StarOrTriangleEnum.Star, GetRecalculatedCoefficientThreePhaseFirstWindingStar },
                {StarOrTriangleEnum.Triangle, GetRecalculatedCoefficientThreePhaseFirstingTriangle }
            };
        }

        public bool TryGetTransformer(out Transformer transformer)
        {
            transformer = null;

            if (TransformerConfig.TransformerType == TransformerTypeEnum.ThreePhase && 
                (TransformerConfig.FirstWinding == StarOrTriangleEnum.None || TransformerConfig.SecondWinding == StarOrTriangleEnum.None)) 
                return false;

            if (U1r <= 0 || U2r <= 0 || I0 <= 0 || I1r <= 0 || U1sc <= 0 || P0 < 0 || Psc < 0 || U1r < U1sc) return false;

            var gain = TransformerTypeRecalculatedCoefficientDictionary[TransformerConfig.TransformerType].Invoke();

            double Z0 = U1r / I0 * gain.Z0_Gain;                                                            //полное сопротивление намагничивающей ветви
            double R0 = P0 / Math.Pow(I0, 2) * gain.R0_Gain;                                                //активное сопротивление намагничивающей ветви
            double X0 = Math.Sqrt(Math.Pow(Z0, 2) - Math.Pow(R0, 2));                                       //реактивное сопротивление намагничивающей ветви
            if (double.IsNaN(X0))
                return false;                                                                               //активная мощность не может быть больше полной мощности (P_Unload < U1_Rated * I1_Unload) 

            double Z_ShortCircuit = U1sc / I1r * gain.Z_ShortCircuit_Gain;                                  //полное сопротивление короткого замыкания
            double R_ShortCircuit = Psc / Math.Pow(I1r, 2) * gain.R_ShortCircuit_Gain;                      //активное сопротивление короткого замыкания
            double X_ShortCircuit = Math.Sqrt(Math.Pow(Z_ShortCircuit, 2) - Math.Pow(R_ShortCircuit, 2));   //реактивное сопротивление короткого замыкания
            if (double.IsNaN(X_ShortCircuit))
                return false;                                                         //активная мощность не может быть больше полной мощности (P_ShortCircuit < U1_ShortCircuit * I1_Rated)

            double R12 = R_ShortCircuit / 2;                                          //активное сопротивление рассеяния
            double X12 = X_ShortCircuit / 2;                                          //реактивное сопротивление рассеяния
            double U1rPhase = ToPhaseVoltage(U1r, TransformerConfig.FirstWinding);
            double U2rPhase = ToPhaseVoltage(U2r, TransformerConfig.SecondWinding);
            double K = U1rPhase / U2rPhase;                                           //коэффициент трансформации

            Complex Zm = new Complex(R0, X0);
            Complex Z1 = new Complex(R12, X12);
            Complex Z2_Corrected = new Complex(R12, X12);

            var equivalentCurcuit = new EquivalentCurcuit { Zm = Zm, Z1 = Z1, Z2_Сorrected = Z2_Corrected, K = K };
            var transformerConfig = new TransformerConfig()
            {
                TransformerType = TransformerConfig.TransformerType,
                FirstWinding = TransformerConfig.FirstWinding,
                SecondWinding = TransformerConfig.SecondWinding
            };

            transformer = new Transformer(equivalentCurcuit, transformerConfig);
            return true;
        }
        private double ToPhaseVoltage(double linearVoltage, StarOrTriangleEnum windingType)
        {
            return linearVoltage * ToPhaseVoltageGain[windingType];
        }
    }
}