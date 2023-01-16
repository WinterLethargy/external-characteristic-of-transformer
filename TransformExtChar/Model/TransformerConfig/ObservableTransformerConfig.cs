using System;
using System.Collections.Generic;
using System.Text;
using TransformExtChar.Infrastructure;

namespace TransformExtChar.Model
{
    public class ObservableTransformerConfig : OnPropertyChangedClass, ITransformerConfig
    {
        #region свойства
        public StarOrTriangleEnum FirstWinding 
        {
            get => _transformerConfig.FirstWinding; 
            set 
            {
                _transformerConfig.FirstWinding = value;
                OnPropertyChanged();
            } 
        }
        public StarOrTriangleEnum SecondWinding
        {
            get => _transformerConfig.SecondWinding;
            set
            {
                _transformerConfig.SecondWinding = value;
                OnPropertyChanged();
            }
        }
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

        public ObservableTransformerConfig() : this(new TransformerConfig()) { }
        public ObservableTransformerConfig(TransformerConfig transformerConfig) => _transformerConfig = transformerConfig;
    }
}
