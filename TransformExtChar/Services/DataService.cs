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
        internal bool TryOpenCSV(out CsvDocument csvDocument, string fileName = null)
        {
            csvDocument = new CsvDocument();

            if (fileName == null && !TryGetFileName(out fileName))
                return false;

            string ext = Path.GetExtension(fileName);
            if (ext != ".csv" && ext != ".txt")
            {
                MessageBox.Show("Недопустимое расширение!", "Недопустимое расширение!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return csvDocument.TryLoad(fileName);
        }
    }
}
