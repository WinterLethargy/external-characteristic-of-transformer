using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using TransformExtChar.Model;

namespace TransformExtChar.Services
{
    internal class DataService
    {
        public bool TryGetFileName(out string path)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "csv|*.csv;*.txt";
            if (dlg.ShowDialog().Value)
            {
                path = dlg.FileName;
                return true;
            }
            else
            {
                path = string.Empty;
                return false;
            }
        }
        internal bool TryOpenCSV(string fileName, out List<VCPointData> points)
        {
            points = new List<VCPointData>();
            string ext = Path.GetExtension(fileName);
            if (ext != ".csv" && ext != ".txt")
            {
                MessageBox.Show("Недопустимое расширение!", "Недопустимое расширение!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            
            using (var r = new StreamReader(fileName))
            {
                var header = r.ReadLine();

                if (!TryGetHadersAndSeparator(out var headers, out var separator) ||
                    headers[0].Equals(headers[1], StringComparison.OrdinalIgnoreCase)||
                    headers.Length > 2)
                {
                    InvalidFormatted();
                    return false;
                }

                if (!UICheck(headers[0], out var firstColumn) & !UICheck(headers[1], out var secondColumn))
                {
                    InvalidFormatted();
                    return false;
                }

                int i, u;

                i = firstColumn == "I" ? 0 : 1;
                u = firstColumn == "U" ? 0 : 1;

                while (!r.EndOfStream)
                {
                    var line = r.ReadLine();
                    if (line == null || line.StartsWith("%") || line.StartsWith("//"))
                        continue;
                    var IUstring = line.Split(separator);
                    try
                    {
                        points.Add(new VCPointData
                        {
                            Current = double.Parse(IUstring[i], CultureInfo.InvariantCulture),
                            Voltage = double.Parse(IUstring[u], CultureInfo.InvariantCulture)
                        });
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Файл неправильно сформатирован!\nЧисла должны быть с разделяющей точкой.", "Не удалось прочесть файл", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                
                return true;

                bool TryGetHadersAndSeparator(out string[] headers, out string separator)
                {
                    var headers1 = header.Split(',');
                    var headers2 = header.Split(";");
                    if (headers1.Length > headers2.Length)
                    {
                        headers = headers1;
                        separator = ",";
                        return true;
                    }
                    else if (headers2.Length > headers1.Length)
                    {
                        headers = headers2;
                        separator = ";";
                        return true;
                    }
                    else
                    {                           // ожидается заголовок типа U,I или I,U
                        headers = null;
                        separator = null;
                        return false;
                    }
                }
                bool UICheck(string UorI, out string result)
                {
                    if (UorI.Equals("U", StringComparison.OrdinalIgnoreCase))
                    {
                        result = "U";
                        return true;
                    }
                    if (UorI.Equals("I", StringComparison.OrdinalIgnoreCase))
                    {
                        result = "I";
                        return true;
                    }
                    result = null;
                    return false;
                }
                void InvalidFormatted()
                {
                    MessageBox.Show("Файл неправильно сформатирован!\nДолжны быть две колонки: I, U.", "Не удалось прочесть файл", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
