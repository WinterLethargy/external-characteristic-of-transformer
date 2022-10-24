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
    public class TransformerDatasheet : OnPropertyChangedClass, IDataErrorInfo
    {
        #region Property
        public TransformerConfig TransformerConfig { get; set; } = new TransformerConfig();

        private double _U1r;

        [JsonProperty(Required = Required.Always)]
        public double U1r
        {
            get => _U1r;
            set
            {
                if (Set(ref _U1r, value))
                {
                    DataErrorChecker.AboveZeroCheck(value, errors);
                    U1sc_Less_U1r_Check();
                    UnloadedActivePowerCheck();
                }
            }
        }

        private double _U2r;

        [JsonProperty(Required = Required.Always)]
        public double U2r
        {
            get => _U2r;
            set
            {
                if (Set(ref _U2r, value))
                    DataErrorChecker.AboveZeroCheck(value, errors);
            }
        }

        private double _I1r;

        [JsonProperty(Required = Required.Always)]
        public double I1r
        {
            get => _I1r;
            set
            {
                if (Set(ref _I1r, value))
                {
                    DataErrorChecker.AboveZeroCheck(value, errors);
                    ShortCircuitActivePowerCheck();
                }
            }
        }

        private double _I0;

        [JsonProperty(Required = Required.Always)]
        public double I0
        {
            get => _I0;
            set
            {
                if (Set(ref _I0, value))
                {
                    DataErrorChecker.AboveZeroCheck(value, errors);
                    DataErrorChecker.AboveOrEqualZeroCheck(value, errors, nameof(I0_Percent));
                    UnloadedActivePowerCheck();
                    OnPropertyChanged(nameof(I0_Percent));
                }
            }
        }
        [JsonIgnore]
        public double I0_Percent
        {
            get
            {
                var result = I0 / I1r * 100;
                if (double.IsNaN(result))
                    return 0;
                return result;
            }
            set
            {
                I0 = value / 100 * I1r;
                DataErrorChecker.AboveOrEqualZeroCheck(value, errors);
                OnPropertyChanged(nameof(I0));
            }
        }

        private double _P0;

        [JsonProperty(Required = Required.Always)]
        public double P0
        {
            get => _P0;
            set
            {
                if (Set(ref _P0, value))
                {
                    DataErrorChecker.AboveOrEqualZeroCheck(value, errors);
                    UnloadedActivePowerCheck();
                }
            }
        }

        private double _U1sc;

        [JsonProperty(Required = Required.Always)]
        public double U1sc
        {
            get => _U1sc;
            set
            {
                if (Set(ref _U1sc, value))
                {
                    DataErrorChecker.AboveZeroCheck(value, errors);
                    DataErrorChecker.AboveOrEqualZeroCheck(value, errors, nameof(U1sc_Percent));
                    U1sc_Less_U1r_Check();
                    ShortCircuitActivePowerCheck();
                    OnPropertyChanged(nameof(U1sc_Percent));
                }
            }
        }

        [JsonIgnore]
        public double U1sc_Percent
        {
            get
            {
                var result = U1sc / U1r * 100;
                if(double.IsNaN(result))
                    return 0;
                return result;
            }
            set
            {
                U1sc = value / 100 * U1r;
                DataErrorChecker.AboveOrEqualZeroCheck(value, errors);
                OnPropertyChanged(nameof(U1sc));
            }
        }

        private double _Psc;

        [JsonProperty(Required = Required.Always)]
        public double Psc
        {
            get => _Psc;
            set
            {
                if (Set(ref _Psc, value))
                {
                    DataErrorChecker.AboveOrEqualZeroCheck(value, errors);
                    ShortCircuitActivePowerCheck();
                }
            }
        }
        #endregion

        #region Реализация IDataErrorInfo
        [JsonIgnore]
        public string Error => errors.Any(str => str.Value != null) ? "Error" : string.Empty;
        public string this[string columnName] => errors.ContainsKey(columnName) ? errors[columnName] : null;

        private Dictionary<string, string> errors = new Dictionary<string, string>();
        #endregion

        #region Проверки ошибок
        private void U1sc_Less_U1r_Check([CallerMemberName] string PropertyName = null, bool recursive = true)
        {
            DataErrorChecker.CheckErrors(() => U1r < U1sc, "Номинальное напряжение не может быть меньше напряжения короткого замыкания", errors, PropertyName);

            if (recursive)
            {
                const string firstName = nameof(U1r);
                const string secondName = nameof(U1sc);
                switch (PropertyName)
                {
                    case firstName:
                        U1sc_Less_U1r_Check(secondName, false);
                        OnPropertyChanged(secondName);
                        break;
                    case secondName:
                        U1sc_Less_U1r_Check(firstName, false);
                        OnPropertyChanged(firstName);
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
        }
        private void UnloadedActivePowerCheck([CallerMemberName] string PropertyName = null, bool recursive = true)
        {
            Func<bool> cheker;
            if(TransformerConfig.TransformerType == TransformerTypeEnum.OnePhase || TransformerConfig.TransformerType == TransformerTypeEnum.None)
            {
                cheker = () => P0 > U1r * I0;
            }
            else if (TransformerConfig.TransformerType == TransformerTypeEnum.ThreePhase)
            {
                cheker = () => P0 > U1r * I0 * Math.Sqrt(3);
            }
            else throw new ArgumentException();

            DataErrorChecker.CheckErrors(cheker, "Активная мощность холостого хода не может быть больше полной мощности холостого хода", errors, PropertyName);

            if (recursive)
            {
                const string firstName = nameof(P0);
                const string secondName = nameof(U1r);
                const string thirdName = nameof(I0);

                switch (PropertyName)
                {
                    case firstName:
                        UnloadedActivePowerCheck(secondName, false);
                        OnPropertyChanged(secondName);
                        UnloadedActivePowerCheck(thirdName, false);
                        OnPropertyChanged(thirdName);
                        break;
                    case secondName:
                        UnloadedActivePowerCheck(firstName, false);
                        OnPropertyChanged(firstName);
                        UnloadedActivePowerCheck(thirdName, false);
                        OnPropertyChanged(thirdName);
                        break;
                    case thirdName:
                        UnloadedActivePowerCheck(firstName, false);
                        OnPropertyChanged(firstName);
                        UnloadedActivePowerCheck(secondName, false);
                        OnPropertyChanged(secondName);
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
        }
        private void ShortCircuitActivePowerCheck([CallerMemberName] string PropertyName = null, bool recursive = true) // НУЖНО ПЕРЕДЕЛАТЬ
        {
            Func<bool> cheker;
            if (TransformerConfig.TransformerType == TransformerTypeEnum.OnePhase || TransformerConfig.TransformerType == TransformerTypeEnum.None)
            {
                cheker = () => Psc > U1sc * I1r;
            }
            else if (TransformerConfig.TransformerType == TransformerTypeEnum.ThreePhase)
            {
                cheker = () => Psc > U1sc * I1r * Math.Sqrt(3);
            }
            else throw new ArgumentException();

            DataErrorChecker.CheckErrors(cheker, "Активная мощность короткого замыкания не может быть больше полной мощности короткого замыкания", errors, PropertyName);

            if (recursive)
            {
                const string firstName = nameof(Psc);
                const string secondName = nameof(U1sc);
                const string thirdName = nameof(I1r);

                switch (PropertyName)
                {
                    case firstName:
                        ShortCircuitActivePowerCheck(secondName, false);
                        OnPropertyChanged(secondName);
                        ShortCircuitActivePowerCheck(thirdName, false);
                        OnPropertyChanged(thirdName);
                        break;
                    case secondName:
                        ShortCircuitActivePowerCheck(firstName, false);
                        OnPropertyChanged(firstName);
                        ShortCircuitActivePowerCheck(thirdName, false);
                        OnPropertyChanged(thirdName);
                        break;
                    case thirdName:
                        ShortCircuitActivePowerCheck(firstName, false);
                        OnPropertyChanged(firstName);
                        ShortCircuitActivePowerCheck(secondName, false);
                        OnPropertyChanged(secondName);
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
        }
        #endregion

        #region словари и методы, возвращающие множители для сопротивлений
        private Dictionary<TransformerTypeEnum, Func<(double Z0_Gain, double R0_Gain, double Z_ShortCircuit_Gain, double R_ShortCircuit_Gain)>> TransformerTypeRecalculatedCoefficientDictionary;

        private Dictionary<StarOrTriangleEnum, Func<(double Z0_Gain, double R0_Gain, double Z_ShortCircuit_Gain, double R_ShortCircuit_Gain)>> StarOrTriangleRecalculatedCoefficientDictionary;
        private (double Z0_Gain, double R0_Gain, double Z_ShortCircuit_Gain, double R_ShortCircuit_Gain) GetRecalculatedCoefficientNone() => (1, 1, 1, 1);
        private (double Z0_Gain, double R0_Gain, double Z_ShortCircuit_Gain, double R_ShortCircuit_Gain) GetRecalculatedCoefficientOnePhase() => (1, 1, 1, 1);
        private (double Z0_Gain, double R0_Gain, double Z_ShortCircuit_Gain, double R_ShortCircuit_Gain) GetRecalculatedCoefficientThreePhase() => StarOrTriangleRecalculatedCoefficientDictionary[TransformerConfig.FirstWinding].Invoke();
        private (double Z0_Gain, double R0_Gain, double Z_ShortCircuit_Gain, double R_ShortCircuit_Gain) GetRecalculatedCoefficientThreePhaseFirstWindingStar() => (1 / Math.Sqrt(3), 1.0 / 3.0, 1 / Math.Sqrt(3), 1.0 / 3.0);
        private (double Z0_Gain, double R0_Gain, double Z_ShortCircuit_Gain, double R_ShortCircuit_Gain) GetRecalculatedCoefficientThreePhaseFirstingTriangle() => (Math.Sqrt(3), 1, Math.Sqrt(3), 1);
        #endregion

        public TransformerDatasheet()
        {
            DataErrorChecker.AboveZeroCheck(U1r, errors, nameof(U1r)); // С этими поляхи должны быть ошибки данных
            DataErrorChecker.AboveZeroCheck(U2r, errors, nameof(U2r));
            DataErrorChecker.AboveZeroCheck(I1r, errors, nameof(I1r));
            DataErrorChecker.AboveZeroCheck(I0, errors, nameof(I0));
            DataErrorChecker.AboveZeroCheck(U1sc, errors, nameof(U1sc));


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

            if (TransformerConfig.TransformerType == TransformerTypeEnum.ThreePhase && TransformerConfig.FirstWinding == StarOrTriangleEnum.None) return false;

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
                return false;                                               //активная мощность не может быть больше полной мощности (P_ShortCircuit < U1_ShortCircuit * I1_Rated)
            double R12 = R_ShortCircuit / 2;                                //активное сопротивление рассеяния
            double X12 = X_ShortCircuit / 2;                                //реактивное сопротивление рассеяния
            double K = U1r / U2r;                                           //коэффициент трансформации

            Complex Zm = new Complex(R0, X0);
            Complex Z1 = new Complex(R12, X12);
            Complex Z2_Corrected = new Complex(R12, X12);

            var equivalentCurcuit = new EquivalentCurcuit { Zm = Zm, Z1 = Z1, Z2_Сorrected = Z2_Corrected, K = K };
            var newTransformerConfig = new TransformerConfig()
            {
                TransformerType = TransformerConfig.TransformerType,
                FirstWinding = TransformerConfig.FirstWinding,
                SecondWinding = TransformerConfig.SecondWinding
            };

            transformer = new Transformer(equivalentCurcuit) { TransformerConfig = newTransformerConfig };
            return true;
        }
    }
}