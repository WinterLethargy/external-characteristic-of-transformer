using System;
using System.Collections.Generic;
using System.Text;
using TransformExtChar.Infrastructure;

namespace TransformExtChar.Model
{
    internal class ObservableTransformerDatasheet : OnPropertyChangedClass, ITransformerDatasheet
    {
        #region свойства
        public double I0 
        { 
            get => _transformerDatasheet.I0;
            set 
            {
                _transformerDatasheet.I0 = value;
                OnPropertyChanged();
            }
        }
        public double I0_Percent
        {
            get => _transformerDatasheet.I0_Percent;
            set
            {
                _transformerDatasheet.I0_Percent = value;
                OnPropertyChanged();
            }
        }
        public double I1r
        {
            get => _transformerDatasheet.I1r;
            set
            {
                _transformerDatasheet.I1r = value;
                OnPropertyChanged();
            }
        }
        public double P0
        {
            get => _transformerDatasheet.P0;
            set
            {
                _transformerDatasheet.P0 = value;
                OnPropertyChanged();
            }
        }
        public double Psc
        {
            get => _transformerDatasheet.Psc;
            set
            {
                _transformerDatasheet.Psc = value;
                OnPropertyChanged();
            }
        }
        public ObservableTransformerConfig TransformerConfig
        {
            get => (ObservableTransformerConfig)_transformerDatasheet.TransformerConfig;
            set
            {
                _transformerDatasheet.TransformerConfig = value;
                OnPropertyChanged();
            }
        }
        ITransformerConfig ITransformerDatasheet.TransformerConfig => _transformerDatasheet.TransformerConfig;

        public double U1r
        {
            get => _transformerDatasheet.U1r;
            set
            {
                _transformerDatasheet.U1r = value;
                OnPropertyChanged();
            }
        }
        public double U1sc
        {
            get => _transformerDatasheet.U1sc;
            set
            {
                _transformerDatasheet.U1sc = value;
                OnPropertyChanged();
            }
        }
        public double U1sc_Percent
        {
            get => _transformerDatasheet.U1sc_Percent;
            set
            {
                _transformerDatasheet.U1sc_Percent = value;
                OnPropertyChanged();
            }
        }
        public double U2r
        {
            get => _transformerDatasheet.U2r;
            set
            {
                _transformerDatasheet.U2r = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private TransformerDatasheet _transformerDatasheet;
        public ObservableTransformerDatasheet() : this(new TransformerDatasheet(new ObservableTransformerConfig())) { }
        public ObservableTransformerDatasheet(TransformerDatasheet transformerDatasheet)
        {
            RegisterCohesionedProperties(nameof(I0), nameof(I0_Percent));
            RegisterCohesionedProperties(nameof(U1sc), nameof(U1sc_Percent));

            _transformerDatasheet = transformerDatasheet;
            
            var transformerConfig = transformerDatasheet.TransformerConfig;

            if(transformerConfig is OnPropertyChangedClass == false)
            {
                var observableTransformerConfig = new ObservableTransformerConfig()
                {
                    FirstWinding = transformerConfig.FirstWinding,
                    SecondWinding = transformerConfig.SecondWinding,
                    TransformerType = transformerConfig.TransformerType
                };
                _transformerDatasheet.TransformerConfig = observableTransformerConfig;
            }
        }
        public bool TryGetTransformer(out Transformer transformer)
        {
            return _transformerDatasheet.TryGetTransformer(out transformer);
        }
    }
}
