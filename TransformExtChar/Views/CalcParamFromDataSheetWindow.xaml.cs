using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TransformExtChar.ViewModel;

namespace TransformExtChar.Views
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class CalcParamFromDataSheetWindow : Window
    {
        public CalcParamFromDataSheetWindow()
        {
            InitializeComponent();
        }
        public void CalcParam(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            DialogResult = true;
            this.Close();
        }
        public void Cancle(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            this.Close();
        }
    }
}
