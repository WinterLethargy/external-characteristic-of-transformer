using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using TransformExtChar.Model;

namespace TransformExtChar.Infrastructure.Converters
{
    public class TransformerTypeConverter : IValueConverter 
    { 
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TransformerTypeEnum transformerType = (TransformerTypeEnum)value;

            string transformerTypeString = string.Empty;

            switch (transformerType)
            {
                case TransformerTypeEnum.None:
                    transformerTypeString = "Не выбрано";
                    break;
                case TransformerTypeEnum.OnePhase:
                    transformerTypeString = "Однофазный трансформатор";
                    break;
                case TransformerTypeEnum.ThreePhase:
                    transformerTypeString = "Трехфазный трансформатор";
                    break;
                default:
                    throw new ArgumentException("В конвертер передан несуществующий тип трансформатора");
            }
            return transformerTypeString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string transformerTypeString = (string)value;

            TransformerTypeEnum transformerType;

            switch (transformerTypeString)
            {
                case "Не выбрано":
                    transformerType = TransformerTypeEnum.None;
                    break;
                case "Однофазный трансформатор":
                    transformerType = TransformerTypeEnum.OnePhase;
                    break;
                case "Трехфазный трансформатор":
                    transformerType = TransformerTypeEnum.ThreePhase;
                    break;
                default:
                    throw new ArgumentException("В конвертер передан несуществующий тип трансформатора");
            }

            return transformerType;
        }
    }
}
