using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransformExtChar.Infrastructure.DataErrorVerify;

namespace TransformExtChar.Model
{
    public class DataErrorInfoTransformerDatasheet : DataErrorInfoClass, ITransformerDatasheet
    {
        #region свойства
        [JsonProperty(Required = Required.Always)]
        public double I0
        {
            get => _transformerDatasheet.I0;
            set
            {
                _transformerDatasheet.I0 = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public double I0_Percent
        {
            get => _transformerDatasheet.I0_Percent;
            set
            {
                _transformerDatasheet.I0_Percent = value;
                OnPropertyChanged();
            }
        }
        [JsonProperty(Required = Required.Always)]
        public double I1r
        {
            get => _transformerDatasheet.I1r;
            set
            {
                _transformerDatasheet.I1r = value;
                OnPropertyChanged();
            }
        }
        [JsonProperty(Required = Required.Always)]
        public double P0
        {
            get => _transformerDatasheet.P0;
            set
            {
                _transformerDatasheet.P0 = value;
                OnPropertyChanged();
            }
        }
        [JsonProperty(Required = Required.Always)]
        public double Psc
        {
            get => _transformerDatasheet.Psc;
            set
            {
                _transformerDatasheet.Psc = value;
                OnPropertyChanged();
            }
        }
        [JsonProperty(Required = Required.Always)]
        public DataErrorInfoTransformerConfig TransformerConfig => (DataErrorInfoTransformerConfig)_transformerDatasheet.TransformerConfig; // сеттер должен предусматривать отписку и подписку событий валидации правил
        ITransformerConfig ITransformerDatasheet.TransformerConfig => _transformerDatasheet.TransformerConfig;

        [JsonProperty(Required = Required.Always)]
        public double U1r
        {
            get => _transformerDatasheet.U1r;
            set
            {
                _transformerDatasheet.U1r = value;
                OnPropertyChanged();
            }
        }
        [JsonProperty(Required = Required.Always)]
        public double U1sc
        {
            get => _transformerDatasheet.U1sc;
            set
            {
                _transformerDatasheet.U1sc = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public double U1sc_Percent
        {
            get => _transformerDatasheet.U1sc_Percent;
            set
            {
                _transformerDatasheet.U1sc_Percent = value;
                OnPropertyChanged();
            }
        }
        [JsonProperty(Required = Required.Always)]
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

        public DataErrorInfoTransformerDatasheet() : this(new TransformerDatasheet(new DataErrorInfoTransformerConfig())) { }
        public DataErrorInfoTransformerDatasheet(TransformerDatasheet transformerDatasheet)
        {
            _transformerDatasheet = transformerDatasheet;

            var transformerConfig = transformerDatasheet.TransformerConfig;

            if (transformerConfig is DataErrorInfoClass == false)
            {
                var observableTransformerConfig = new DataErrorInfoTransformerConfig()
                {
                    FirstWinding = transformerConfig.FirstWinding,
                    SecondWinding = transformerConfig.SecondWinding,
                    TransformerType = transformerConfig.TransformerType
                };
                _transformerDatasheet.TransformerConfig = observableTransformerConfig;
            }

            RegisterRule_PropMustBeAboveZero(() => U1r);
            RegisterRule(() => U1r > U1sc, "Номинальное напряжение не может быть меньше напряжения короткого замыкания");
            RegisterRule_PropMustBeAboveZero(() => U2r);
            RegisterRule_PropMustBeAboveZero(() => I1r);

            RegisterCohesionedProperties(nameof(I0), nameof(I0_Percent));
            var I0AboveZeroRule = RegisterRule_PropMustBeAboveZero(() => I0);
            var I0_PercentAboveOrEaualZeroRule = RegisterRule_PropMustBeAboveOrEqualZero(() => I0_Percent);
            I0AboveZeroRule.Validate += (sender, args) => CheckRuleAndSetError(I0_PercentAboveOrEaualZeroRule, false);
            I0_PercentAboveOrEaualZeroRule.Validate += (sender, args) => CheckRuleAndSetError(I0AboveZeroRule, false);

            RegisterRule_PropMustBeAboveOrEqualZero(() => P0);

            RegisterCohesionedProperties(nameof(U1sc), nameof(U1sc_Percent));
            var U1scAboveZeroRule = RegisterRule_PropMustBeAboveZero(() => U1sc);
            var U1sc_PercentAboveOrEaualZeroRule = RegisterRule_PropMustBeAboveOrEqualZero(() => U1sc_Percent);
            U1scAboveZeroRule.Validate += (sender, args) => CheckRuleAndSetError(U1sc_PercentAboveOrEaualZeroRule, false);
            U1sc_PercentAboveOrEaualZeroRule.Validate += (sender, args) => CheckRuleAndSetError(U1scAboveZeroRule, false);

            RegisterRule_PropMustBeAboveOrEqualZero(() => Psc);

            Func<bool> UnloadedActivePowerRule = () =>
            {
                if (TransformerConfig.TransformerType == TransformerTypeEnum.OnePhase || TransformerConfig.TransformerType == TransformerTypeEnum.None)
                {
                    return P0 < U1r * I0;
                }
                else if (TransformerConfig.TransformerType == TransformerTypeEnum.ThreePhase)
                {
                    return P0 < U1r * I0 * Math.Sqrt(3);
                }
                else throw new ArgumentException();
            };
            var DSUnActPowlinkedPropertyNames = new string[] { nameof(P0), nameof(U1r), nameof(I0) };
            var TCUnActPowlinkedPropertyNames = new string[] { nameof(TransformerConfig.TransformerType) };
            var UnloadedActivePowerRuleText = "Активная мощность холостого хода не может быть больше полной мощности холостого хода";

            var DSUnloadedActivePowerRule = RegisterRule(
                UnloadedActivePowerRule,
                UnloadedActivePowerRuleText,
                DSUnActPowlinkedPropertyNames);
            var TCUnloadedActivePowerRule = TransformerConfig.RegisterRule(
                UnloadedActivePowerRule,
                UnloadedActivePowerRuleText,
                TCUnActPowlinkedPropertyNames);

            DSUnloadedActivePowerRule.Validate += (sender, args) =>
            {
                TransformerConfig.CheckRuleAndSetError(TCUnloadedActivePowerRule, false);
                TransformerConfig.DefaultPropertyChanged(TCUnActPowlinkedPropertyNames);
            };
            TCUnloadedActivePowerRule.Validate += (sender, args) =>
            {
                CheckRuleAndSetError(DSUnloadedActivePowerRule, false);
                DefaultPropertyChanged(DSUnActPowlinkedPropertyNames);
            };

            Func<bool> ShortCircuitActivePowerRule = () =>
            {
                if (TransformerConfig.TransformerType == TransformerTypeEnum.OnePhase || TransformerConfig.TransformerType == TransformerTypeEnum.None)
                {
                    return Psc < U1sc * I1r;
                }
                else if (TransformerConfig.TransformerType == TransformerTypeEnum.ThreePhase)
                {
                    return Psc < U1sc * I1r * Math.Sqrt(3);
                }
                else throw new ArgumentException();
            };
            var DSShCrcActPowlinkedPropertyNames = new string[] { nameof(Psc), nameof(U1sc), nameof(I1r) };
            var TCShCrcActPowlinkedPropertyNames = new string[] { nameof(TransformerConfig.TransformerType) };
            var ShortCircuitActivePowerRuleText = "Активная мощность короткого замыкания не может быть больше полной мощности короткого замыкания";

            var DSShortCircuitActivePowerRule = RegisterRule(
                ShortCircuitActivePowerRule,
                ShortCircuitActivePowerRuleText,
                DSShCrcActPowlinkedPropertyNames);
            var TCShortCircuitActivePowerRule = TransformerConfig.RegisterRule(
                ShortCircuitActivePowerRule,
                ShortCircuitActivePowerRuleText,
                TCShCrcActPowlinkedPropertyNames);

            DSShortCircuitActivePowerRule.Validate += (sender, args) =>
            {
                TransformerConfig.CheckRuleAndSetError(TCShortCircuitActivePowerRule, false);
                TransformerConfig.DefaultPropertyChanged(TCShCrcActPowlinkedPropertyNames);
            };
            TCShortCircuitActivePowerRule.Validate += (sender, args) =>
            {
                CheckRuleAndSetError(DSShortCircuitActivePowerRule, false);
                DefaultPropertyChanged(DSShCrcActPowlinkedPropertyNames);
            };

            CheckAllRulesAndSetError();
            var propNames = GetType().GetProperties().Select(p => p.Name);
            DefaultPropertyChanged(propNames);
        }

        public bool TryGetTransformer(out Transformer transformer)
        {
            return _transformerDatasheet.TryGetTransformer(out transformer);
        }
    }
}