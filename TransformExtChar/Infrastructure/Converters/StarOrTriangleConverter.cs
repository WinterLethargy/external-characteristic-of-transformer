using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using TransformExtChar.Model;

namespace TransformExtChar.Infrastructure.Converters
{
    public class StarOrTriangleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StarOrTriangleEnum StarOrTriangleType = (StarOrTriangleEnum)value;

            string StarOrTriangleString = string.Empty;

            switch (StarOrTriangleType)
            {
                case StarOrTriangleEnum.None:
                    StarOrTriangleString = "Не выбрано";
                    break;
                case StarOrTriangleEnum.Star:
                    StarOrTriangleString = "Звезда";
                    break;
                case StarOrTriangleEnum.Triangle:
                    StarOrTriangleString = "Треугольник";
                    break;
                default:
                    throw new ArgumentException("В конвертер передан несуществующий тип соединения обмоток");
            }
            return StarOrTriangleString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string StarOrTriangleString = (string)value;

            StarOrTriangleEnum StarOrTriangleType;

            switch (StarOrTriangleString)
            {
                case "Не выбрано":
                    StarOrTriangleType = StarOrTriangleEnum.None;
                    break;
                case "Звезда":
                    StarOrTriangleType = StarOrTriangleEnum.Star;
                    break;
                case "Треугольник":
                    StarOrTriangleType = StarOrTriangleEnum.Triangle;
                    break;
                default:
                    throw new ArgumentException("В конвертер передан несуществующий тип соединения обмоток");
            }

            return StarOrTriangleType;
        }
    }
}
