using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace TransformExtChar.Infrastructure.Converters
{
    public class FloatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value).ToString("0.###");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // return an invalid value in case of the value ends with a point
            string input = value.ToString();

            double result;

            if(double.TryParse(input, out result) || double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                return result;

            return "";
        }
    }
}
