using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TransformExtChar.Services
{
    internal class GetDataService
    {
        internal void OpenCSV(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            if (ext != ".csv" || ext != ".txt") return;

            using (var r = new StreamReader(fileName))
            {
                var header = r.ReadLine();

                if (!TryGetHadersAndSeparator(out var headers, out var separator))
                    return;

                if (!UICheck(headers[0], out var firstColum))
                    return;

                if (!UICheck(headers[1], out var secondColumn))
                    return;

                var Items = new List<string[]>();

                while (!r.EndOfStream)
                {
                    var line = r.ReadLine();
                    if (line == null || line.StartsWith("%") || line.StartsWith("//"))
                        continue;
                    Items.Add(line.Split(separator));
                }

                // ОСТАНОВИЛСЯ ЗДЕСЬ

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
            }

        }
    }
}
