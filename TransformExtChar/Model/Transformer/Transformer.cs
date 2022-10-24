using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TransformExtChar.Infrastructure;

namespace TransformExtChar.Model
{
    public class Transformer : ITransformer
    {
        #region Свойства и поля
        public IEquivalentCurcuit EquivalentCurcuit { get; set; }
        public ITransformerConfig TransformerConfig { get; set; }
        #endregion

        public Transformer() : this(new EquivalentCurcuit(), new TransformerConfig()) { }
        public Transformer(IEquivalentCurcuit equivalentCurcuit, ITransformerConfig transformerConfig)
        {
            EquivalentCurcuit = equivalentCurcuit;
            TransformerConfig = transformerConfig;

            TransformerTypeRecalculatedCoefficientDictionary = new Dictionary<TransformerTypeEnum, Func<(double VoltageGain, double CurrentGane)>>()
            {
                { TransformerTypeEnum.None, GetRecalculatedCoefficientNone },
                { TransformerTypeEnum.OnePhase, GetRecalculatedCoefficientOnePhase },
                { TransformerTypeEnum.ThreePhase,  GetRecalculatedCoefficientThreePhase }
            };

            StarOrTriangleRecalculatedCoefficientDictionary = new Dictionary<StarOrTriangleEnum, Func<(double VolageGain, double CurrentGane)>>()
            {
                {StarOrTriangleEnum.Star, GetRecalculatedCoefficientThreePhaseSecondWindingStar },
                {StarOrTriangleEnum.Triangle, GetRecalculatedCoefficientThreePhaseSecondWindingTriangle }
            };
        }

        #region Методы и поля для пересчета точек

        private Dictionary<TransformerTypeEnum, Func<(double VoltageGain, double CurrentGane)>> TransformerTypeRecalculatedCoefficientDictionary;

        private Dictionary<StarOrTriangleEnum, Func<(double VoltageGain, double CurrentGane)>> StarOrTriangleRecalculatedCoefficientDictionary;
        private (double VoltageGain, double CurrentGane) GetRecalculatedCoefficientNone() => (1, 1);
        private (double VoltageGain, double CurrentGane) GetRecalculatedCoefficientOnePhase() => (1 / EquivalentCurcuit.K, EquivalentCurcuit.K);
        private (double VoltageGain, double CurrentGane) GetRecalculatedCoefficientThreePhase() => StarOrTriangleRecalculatedCoefficientDictionary[TransformerConfig.SecondWinding].Invoke();
        private (double VoltageGain, double CurrentGane) GetRecalculatedCoefficientThreePhaseSecondWindingStar() => (1 / EquivalentCurcuit.K * Math.Sqrt(3), EquivalentCurcuit.K);
        private (double VoltageGain, double CurrentGane) GetRecalculatedCoefficientThreePhaseSecondWindingTriangle() => (1 / EquivalentCurcuit.K, EquivalentCurcuit.K * Math.Sqrt(3));

        #endregion

        public List<VCPointData> GetExternalCharacteristic(double fi2_rad = 0, double I2_correctedStart = 0, double I2_correctedEnd = 0, double U1 = 0, double I2_step = 0.01)
        {
            if (TransformerConfig.TransformerType == TransformerTypeEnum.ThreePhase && TransformerConfig.SecondWinding == StarOrTriangleEnum.None) return new List<VCPointData>();

            var gain = TransformerTypeRecalculatedCoefficientDictionary[TransformerConfig.TransformerType].Invoke();

            var phaseVoltageGain = TransformerConfig.TransformerType == TransformerTypeEnum.ThreePhase && TransformerConfig.FirstWinding == StarOrTriangleEnum.Star ?
                                   1 / Math.Sqrt(3) :
                                   1;

            return EquivalentCurcuit?.GetExternalCharacteristic(fi2_rad, I2_correctedStart / gain.CurrentGane, I2_correctedEnd / gain.CurrentGane, U1 * phaseVoltageGain, I2_step / gain.CurrentGane).
                                     Select(point =>
                                     {
                                         return new VCPointData
                                         {
                                             Current = point.Current * gain.CurrentGane,
                                             Voltage = point.Voltage * gain.VoltageGain
                                         };
                                     }).
                                     ToList() ?? new List<VCPointData>();
        }
    }
}