using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TransformExtChar.Infrastructure.DataErrorVerify;

namespace TransformExtChar.Model
{
    internal class DataErrorInfoTransformer : DataErrorInfoClass, ITransformer
    {
        #region свойства

        [JsonProperty(Required = Required.Always)]
        public DataErrorInfoEquivalentCurcuit EquivalentCurcuit
        {
            get => (DataErrorInfoEquivalentCurcuit)_transformer.EquivalentCurcuit;
            set
            {
                _transformer.EquivalentCurcuit = value;
                OnPropertyChanged();
            }
        }
        IEquivalentCurcuit ITransformer.EquivalentCurcuit => _transformer.EquivalentCurcuit;

        [JsonProperty(Required = Required.Always)]
        public DataErrorInfoTransformerConfig TransformerConfig
        {
            get => (DataErrorInfoTransformerConfig)_transformer.TransformerConfig;
            set
            {
                _transformer.TransformerConfig = value;
                OnPropertyChanged();
            }
        }
        ITransformerConfig ITransformer.TransformerConfig => _transformer.TransformerConfig;
        #endregion

        private Transformer _transformer;

        public DataErrorInfoTransformer() : this(new Transformer(new DataErrorInfoEquivalentCurcuit(), new DataErrorInfoTransformerConfig())) { }
        public DataErrorInfoTransformer(Transformer transformer)
        {
            _transformer = transformer;
            var equivalentCurcuit = _transformer.EquivalentCurcuit;
            if (equivalentCurcuit is DataErrorInfoClass == false)
            {
                var dataErrorInfoEquivalentCurcuit = new DataErrorInfoEquivalentCurcuit()
                {
                    K = equivalentCurcuit.K,
                    Z1 = equivalentCurcuit.Z1,
                    Z2_Сorrected = equivalentCurcuit.Z2_Сorrected,
                    Zm = equivalentCurcuit.Zm
                };
                _transformer.EquivalentCurcuit = dataErrorInfoEquivalentCurcuit;
            }

            var transformerConfig = _transformer.TransformerConfig;
            if (transformerConfig is DataErrorInfoClass == false)
            {
                var dataErrorInfoTransformerConfig = new DataErrorInfoTransformerConfig()
                {
                    FirstWinding = transformerConfig.FirstWinding,
                    SecondWinding = transformerConfig.SecondWinding,
                    TransformerType = transformerConfig.TransformerType
                };
                _transformer.TransformerConfig = dataErrorInfoTransformerConfig;
            }
        }
        public List<VCPointData> GetExternalCharacteristic(double fi2_rad = 0, double I2_correctedStart = 0, double I2_correctedEnd = 0, double U1 = 0, double I2_step = 0.01)
        {
            return _transformer.GetExternalCharacteristic(fi2_rad, I2_correctedStart, I2_correctedEnd, U1, I2_step);
        }
    }
}
