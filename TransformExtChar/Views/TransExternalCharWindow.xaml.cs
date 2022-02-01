using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TransformExtChar.ViewModel;
using TransformExtChar.Views;

namespace TransformExtChar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TransExternalCharWindow : Window
    {
        public TransExternalCharWindow()
        {
            InitializeComponent();
            PlotLineSeries.TrackerFormatString = "{1}: {2:.###}\n{3}: {4:.###}";
        }

        #region обработчики команд
        public void CalcExtChar(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            var thisViewModel = (TransExternalCharViewModel)DataContext;
            thisViewModel.CalcExtChar();
            CurrentAxis.MinimumRange = thisViewModel.I2_step;    // нельзя приблизить больше, чем на шаг тока (избегаем подтормаживания)
            VolageAxis.MinimumRange = thisViewModel.U1 / 100000; // нельзя приблизить больше, чем в сто тысяч раз раз (избегаем подтормаживания)
                                                                 // про MaximumRange сложно что-то сказать
            
        }
        public void CalcParamFromDataSheet(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            CalcParamFromDataSheetWindow dialog = new CalcParamFromDataSheetWindow(); // Создать в другом потоке нельзя
            if (dialog.ShowDialog() == true)                                          // Выполнить метод в другом потоке нельзя
            {
                CalcParamFromDataSheetViewModel calcParamFromDataSheetViewModel = (CalcParamFromDataSheetViewModel)dialog.DataContext;
                TransExternalCharViewModel transExternalCharViewModel = (TransExternalCharViewModel)this.DataContext;
                
                Task.Run(() => transExternalCharViewModel.CalcParamFromDataSheet(calcParamFromDataSheetViewModel)); 
            }
        }
        #endregion
    }
}

//(U1TextBox);
//(Fi2TextBox);
//(KTextBox);
//(R1TextBox);
//(X1TextBox);
//(R2TextBox);
//(X2TextBox);
//(R0TextBox);
//(X0TextBox);