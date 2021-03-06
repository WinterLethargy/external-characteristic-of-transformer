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
    public class Transformer : OnPropertyChangedClass
    {
        #region Свойства и поля

        private EquivalentCurcuit _equivalentCurcuit;
        [JsonProperty(Required = Required.Always)]
        public EquivalentCurcuit EquivalentCurcuit
        {
            get => _equivalentCurcuit;
            set => Set(ref _equivalentCurcuit, value);
        }

        private TransformerTypeEnum _transformerType;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(Required = Required.Always)]
        public TransformerTypeEnum TransformerType
        {
            get => _transformerType;
            set => Set(ref _transformerType, value);
        }

        private StarOrTriangleEnum _firstWinding;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(Required = Required.Always)]
        public StarOrTriangleEnum FirstWinding
        {
            get => _firstWinding;
            set => Set(ref _firstWinding, value);
        }

        private StarOrTriangleEnum _secondWinding;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(Required = Required.Always)]
        public StarOrTriangleEnum SecondWinding
        {
            get => _secondWinding;
            set => Set(ref _secondWinding, value);
        }
        #endregion

        #region Методы и поля для пересчета точек
        private Dictionary<TransformerTypeEnum, Func<(double VolageGain, double CurrentGane)>> TransformerTypeRecalculatedCoefficientDictionary;

        private Dictionary<StarOrTriangleEnum, Func<(double VolageGain, double CurrentGane)>> StarOrTriangleRecalculatedCoefficientDictionary;
        private (double VoltageGain, double CurrentGane) GetRecalculatedCoefficientNone() => (1, 1);
        private (double VoltageGain, double CurrentGane) GetRecalculatedCoefficientOnePhase() => (1 / EquivalentCurcuit.K, EquivalentCurcuit.K);
        private (double VoltageGain, double CurrentGane) GetRecalculatedCoefficientThreePhase() => StarOrTriangleRecalculatedCoefficientDictionary[SecondWinding].Invoke();
        private (double VoltageGain, double CurrentGane) GetRecalculatedCoefficientThreePhaseSecondWindingStar() => (1 / EquivalentCurcuit.K * Math.Sqrt(3), 1 / EquivalentCurcuit.K);
        private (double VoltageGain, double CurrentGane) GetRecalculatedCoefficientThreePhaseSecondWindingTriangle() => (1 / EquivalentCurcuit.K, EquivalentCurcuit.K * Math.Sqrt(3));
        #endregion

        public Transformer(EquivalentCurcuit equivalentCurcuit)
        {
            EquivalentCurcuit = equivalentCurcuit;

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

        public List<VCPointData> GetExternalCharacteristic(double fi2_rad = 0, double I2_correctedStart = 0, double I2_correctedEnd = 0, double U1 = 0, double I2_step = 0.01)
        {
            if (TransformerType == TransformerTypeEnum.ThreePhase && SecondWinding == StarOrTriangleEnum.None) return new List<VCPointData>();

            var gain = TransformerTypeRecalculatedCoefficientDictionary[TransformerType].Invoke();

            return EquivalentCurcuit?.GetExternalCharacteristic(fi2_rad, I2_correctedStart / gain.CurrentGane, I2_correctedEnd / gain.CurrentGane, U1, I2_step / gain.CurrentGane).
                                     Select(point =>
                                     {
                                         return new VCPointData
                                         {
                                             Current = point.Current * gain.CurrentGane,
                                             Voltage = point.Voltage * gain.VolageGain
                                         };
                                     }).
                                     ToList() ?? new List<VCPointData>();
        }
    }
}
