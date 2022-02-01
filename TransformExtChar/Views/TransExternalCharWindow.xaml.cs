using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// 

    // View реализует INotifyPropertyChanged, потому что всё, связанное с Plotter,
    // является логикой отображения, а не данных, но свойство Plotter связано 
    // с XAML файлом посредством привязки данных. С другой стороны Plotter можно
    // сделать статическим, но не думаю, что это хорошая идея.
    public partial class TransExternalCharWindow : Window, INotifyPropertyChanged
    {
        #region Реализация INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }
        #endregion

        private readonly TransExternalCharViewModel ThisViewModel;

        private PlotModel _plotter;
        public PlotModel Plotter { get => _plotter; set => Set(ref _plotter, value); }

        public TransExternalCharWindow()
        {
            InitializeComponent();
            ThisViewModel = (TransExternalCharViewModel)DataContext;
            Plotter = CreatePlotModel();
        }

        private static PlotModel CreatePlotModel()
        {
            PlotModel pm = new PlotModel
            {
                Title = "Внешняя характеристика трансформатора",
                TitleFontSize = 16,
                TitlePadding = 3
            };
            LinearAxis U2 = new LinearAxis
            {
                Title = "U2",
                Unit = "В",
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            };
            LinearAxis I2 = new LinearAxis
            {
                Title = "I2",
                Unit = "А",
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            };
            pm.Axes.Add(U2);
            pm.Axes.Add(I2);
            LineSeries extCh = new LineSeries
            {
                Color = OxyColors.Black,
                TrackerFormatString = "{1}: {2:.###}\n{3}: {4:.###}"
            };
            pm.Series.Add(extCh);
            return pm;
        }

        #region обработчики команд
        async public void CalcExtChar(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            await Task.Run(() => ThisViewModel.CalcExtChar());
            UpdateExtCharSeriesAsync();
        }

        private Task UpdateExtCharSeriesAsync()
        {
            return Task.Run(() => {
                var seriesPoints = ((LineSeries)Plotter.Series[0]).Points;

                seriesPoints.Clear();
                foreach (var point in ThisViewModel.TransExtChar)
                {
                    seriesPoints.Add(new DataPoint(point.Current, point.Voltage));
                }

                Plotter.Axes[0].MinimumRange = ThisViewModel.I2_step;      // нельзя приблизить больше, чем на шаг тока (избегаем подтормаживания)
                Plotter.Axes[1].MinimumRange = ThisViewModel.U1 / 100000;  // нельзя приблизить больше, чем в сто тысяч раз раз (избегаем подтормаживания)
                                                                           // про MaximumRange сложно что-то сказать
                plotter.InvalidatePlot();
            });
        }

        public void CalcParamFromDataSheet(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            CalcParamFromDataSheetWindow dialog = new CalcParamFromDataSheetWindow(); // Создать в другом потоке нельзя
            if (dialog.ShowDialog() == true)                                          // Выполнить метод в другом потоке нельзя
            {
                CalcParamFromDataSheetViewModel calcParamFromDataSheetViewModel = (CalcParamFromDataSheetViewModel)dialog.DataContext;

                Task.Run(() => ThisViewModel.CalcParamFromDataSheet(calcParamFromDataSheetViewModel)); 
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