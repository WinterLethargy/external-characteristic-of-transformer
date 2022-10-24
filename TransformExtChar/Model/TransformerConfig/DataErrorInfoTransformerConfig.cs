using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransformExtChar.Infrastructure.DataErrorVerify;

namespace TransformExtChar.Model
{
    public class DataErrorInfoTransformerConfig : DataErrorInfoClass, ITransformerConfig
    {
        #region свойства
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(Required = Required.Always)]
        public StarOrTriangleEnum FirstWinding
        {
            get => _transformerConfig.FirstWinding;
            set
            {
                _transformerConfig.FirstWinding = value;
                OnPropertyChanged();
            }
        }
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(Required = Required.Always)]
        public StarOrTriangleEnum SecondWinding
        {
            get => _transformerConfig.SecondWinding;
            set
            {
                _transformerConfig.SecondWinding = value;
                OnPropertyChanged();
            }
        }
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(Required = Required.Always)]
        public TransformerTypeEnum TransformerType
        {
            get => _transformerConfig.TransformerType;
            set
            {
                _transformerConfig.TransformerType = value;
                OnPropertyChanged();
            }
        }
        #endregion
        

        private TransformerConfig _transformerConfig;

        public DataErrorInfoTransformerConfig() : this(new TransformerConfig()) { }
        public DataErrorInfoTransformerConfig(TransformerConfig transformerConfig)
        {
            _transformerConfig = transformerConfig;
            RegisterRule(() => 
                (TransformerType == TransformerTypeEnum.ThreePhase && (FirstWinding != StarOrTriangleEnum.None && SecondWinding != StarOrTriangleEnum.None)) ||
                (TransformerType != TransformerTypeEnum.ThreePhase && (FirstWinding == StarOrTriangleEnum.None && SecondWinding == StarOrTriangleEnum.None)), 
                "Невозможное сочетание типа трансформатора и соединения обмоток");

            CheckAllRulesAndSetError();
            var propNames = GetType().GetProperties().Select(p => p.Name);
            DefaultPropertyChanged(propNames);
        }
    }
}
