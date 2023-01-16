using System;
using System.Collections.Generic;
using System.Text;
using TransformExtChar.Infrastructure;

namespace TransformExtChar.Model
{
    public class ObservableTransformer : OnPropertyChangedClass, ITransformer
    {
        #region свойства
        public ObservableEquivalentCurcuit EquivalentCurcuit 
        {
            get => (ObservableEquivalentCurcuit)_transformer.EquivalentCurcuit;
            set
            {
                _transformer.EquivalentCurcuit = value;
                OnPropertyChanged();
            }
        } 
        IEquivalentCurcuit ITransformer.EquivalentCurcuit => _transformer.EquivalentCurcuit;
        public ObservableTransformerConfig TransformerConfig 
        {
            get => (ObservableTransformerConfig)_transformer.TransformerConfig;
            set 
            {
                _transformer.TransformerConfig = value;
                OnPropertyChanged();
            }
        }
        ITransformerConfig ITransformer.TransformerConfig => _transformer.TransformerConfig;
        #endregion

        private Transformer _transformer;

        public ObservableTransformer() : this(new Transformer(new ObservableEquivalentCurcuit(), new ObservableTransformerConfig())) { }
        public ObservableTransformer(Transformer transformer)
        {
            _transformer = transformer;
            var equivalentCurcuit = _transformer.EquivalentCurcuit;
            if (equivalentCurcuit is OnPropertyChangedClass == false)
            {
                var observableEquivalentCurcuit = new ObservableEquivalentCurcuit()
                {
                    K = equivalentCurcuit.K,
                    Z1 = equivalentCurcuit.Z1,
                    Z2_Сorrected = equivalentCurcuit.Z2_Сorrected,
                    Zm = equivalentCurcuit.Zm
                };
                _transformer.EquivalentCurcuit = observableEquivalentCurcuit;
            }
            
            var transformerConfig = _transformer.TransformerConfig;
            if (transformerConfig is OnPropertyChangedClass == false)
            {
                var observableTransformerConfig = new ObservableTransformerConfig()
                {
                    FirstWinding = transformerConfig.FirstWinding,
                    SecondWinding = transformerConfig.SecondWinding,
                    TransformerType = transformerConfig.TransformerType
                };
                _transformer.TransformerConfig = observableTransformerConfig;
            }
        }
        public List<VCPointData> GetExternalCharacteristic(double fi2_rad = 0, double I2_correctedStart = 0, double I2_correctedEnd = 0, double U1 = 0, double I2_step = 0.01)
        {
            return _transformer.GetExternalCharacteristic(fi2_rad, I2_correctedStart, I2_correctedEnd, U1, I2_step);
        }
    }
}
