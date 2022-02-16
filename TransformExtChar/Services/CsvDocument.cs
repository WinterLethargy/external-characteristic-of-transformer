using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace TransformExtChar.Services
{
    public class CsvDocument
    {
        public List<string> Headers { get; private set; }
        public List<string[]> Items { get; private set; }

        public bool TryLoad(string fileName, char separator = '\0')
        {
            using (var r = new StreamReader(fileName))
            {
                var header = r.ReadLine();

                if (!TryGetHadersAndSeparator(out var headers, out separator))
                {
                    InvalidFormatted();
                    return false;
                }

                var protHeaders = header.Split(separator);
                var FileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

                Headers = new List<string>(protHeaders);
                Items = new List<string[]>();

                while (!r.EndOfStream)
                {
                    var line = r.ReadLine();
                    if (line == null || line.StartsWith("%") || line.StartsWith("//"))
                        continue;
                    Items.Add(line.Split(separator));
                }

                return true;

                bool TryGetHadersAndSeparator(out string[] headers, out char separator)
                {
                    var comaCount = Count(header, ',');
                    var semicolonCount = Count(header, ';');
                    if (comaCount > semicolonCount)
                    {
                        headers = header.Split(',');
                        separator = ',';
                        return true;
                    }
                    else if (semicolonCount > comaCount)
                    {
                        headers = header.Split(";");
                        separator = ';';
                        return true;
                    }
                    else
                    {
                        headers = null;
                        separator = '\0';
                        return false;
                    }
                    int Count(string s, char c)
                    {
                        return s.Count(ch => ch == c);
                    }
                }
                void InvalidFormatted()
                {
                    MessageBox.Show("Файл неправильно сформатирован!", "Не удалось прочесть файл", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public IEnumerable<(string title, List<Point> points)> GetData()
        {
            var x = new double[Items.Count];

            for (int i = 0; i < Items.Count; i++)
            {
                x[i] = ParseDouble(Items[i][0]);
            }

            for (int j = 1; j < Headers.Count; j++)  // j столбец
            {
                var list = new List<Point>(Items.Count);

                for (int i = 0; i < Items.Count; i++)  // i строка
                {
                    double y = ParseDouble(Items[i][j]);
                    list.Add(new Point(x[i], y));
                }

                yield return (Headers[j], list);
            }

             double ParseDouble(string s)
             {
                 if (s == null)
                 {
                     return double.NaN;
                 }
                 s = s.Replace(',', '.');
                 double result;
                 if (double.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
                 {
                     return result;
                 }
                 
                 return double.NaN;
             }
        }
    }
}
