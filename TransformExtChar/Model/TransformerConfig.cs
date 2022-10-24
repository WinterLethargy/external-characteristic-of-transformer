using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TransformExtChar.Infrastructure;

namespace TransformExtChar.Model
{
    public class TransformerConfig : OnPropertyChangedClass, IDataErrorInfo
    {
        #region Тип трансформатора и соединение обмоток
        private TransformerTypeEnum _transformerType;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(Required = Required.Always)]
        public TransformerTypeEnum TransformerType
        {
            get => _transformerType;
            set
            {
                Set(ref _transformerType, value);
                TransformerConfigCheck();
            }
        }

        private StarOrTriangleEnum _firstWinding;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(Required = Required.Always)]
        public StarOrTriangleEnum FirstWinding
        {
            get => _firstWinding;
            set
            {
                Set(ref _firstWinding, value);
                TransformerConfigCheck();
            }
        }

        private StarOrTriangleEnum _secondWinding;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(Required = Required.Always)]
        public StarOrTriangleEnum SecondWinding
        {
            get => _secondWinding;
            set
            {
                Set(ref _secondWinding, value);
                TransformerConfigCheck();
            }
        }
        #endregion

        private void TransformerConfigCheck([CallerMemberName] string PropertyName = null, bool recursive = true)
        {
            DataErrorChecker.CheckErrors(() => CheckThreePhase() || CheckAllOther(), "Невозможное сочетание типа трансформатора и соединения обмоток", errors, PropertyName);

            bool CheckThreePhase()
            {
                return TransformerType == TransformerTypeEnum.ThreePhase && (FirstWinding == StarOrTriangleEnum.None || SecondWinding == StarOrTriangleEnum.None);
            }
            bool CheckAllOther()
            {
                return TransformerType != TransformerTypeEnum.ThreePhase && (FirstWinding != StarOrTriangleEnum.None || SecondWinding != StarOrTriangleEnum.None);
            }

            if (recursive)
            {
                const string firstName = nameof(TransformerType);
                const string secondName = nameof(FirstWinding);
                const string thirdName = nameof(SecondWinding);

                switch (PropertyName)
                {
                    case firstName:
                        TransformerConfigCheck(secondName, false);
                        OnPropertyChanged(secondName);
                        TransformerConfigCheck(thirdName, false);
                        OnPropertyChanged(thirdName);
                        break;
                    case secondName:
                        TransformerConfigCheck(firstName, false);
                        OnPropertyChanged(firstName);
                        TransformerConfigCheck(thirdName, false);
                        OnPropertyChanged(thirdName);
                        break;
                    case thirdName:
                        TransformerConfigCheck(firstName, false);
                        OnPropertyChanged(firstName);
                        TransformerConfigCheck(secondName, false);
                        OnPropertyChanged(secondName);
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
        }

        #region Реализация IDataErrorInfo
        [JsonIgnore]
        public string Error => errors.Any(str => str.Value != null) ? "Error" : string.Empty;
        public string this[string columnName] => errors.ContainsKey(columnName) ? errors[columnName] : null;
        private Dictionary<string, string> errors = new Dictionary<string, string>();
        #endregion
    }
}
