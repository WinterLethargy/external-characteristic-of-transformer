using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
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
        private static RoutedCommand _FixSeriesCommand;
        public static RoutedCommand FixSeriesCommand { get { return _FixSeriesCommand; } }

        private static RoutedCommand _DeleteHiddenSeriesCommand;
        public static RoutedCommand DeleteHiddenSeriesCommand { get { return _DeleteHiddenSeriesCommand; } }
        
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
        private LineSeries EditedSeries { get; set; } // График, который будет изменяться при нажатии на "Построить..."
        private LinearAxis UAxis { get; }
        private LinearAxis IAxis { get; }
        private int CalcSeriesCounter { get; set; } = 1;
        private int UsersSeriesCounter { get; set; } = 1;

        #region Конструкторы
        public TransExternalCharWindow()
        {
            InitializeComponent();
            ThisViewModel = (TransExternalCharViewModel)DataContext;
            Plotter = CreatePlotModel();
            UAxis = (LinearAxis)Plotter.Axes[0];
            IAxis = (LinearAxis)Plotter.Axes[1];
            EditedSeries = new LineSeries
            {
                Title = "Рассчетная характеристика",                        // отличитильные черты редактируемого графика - название и цвет
                Color = OxyColors.Black,
                TrackerFormatString = "{1}: {2:.###}\n{3}: {4:.###}"
            };
        }
        static TransExternalCharWindow()
        {
            _FixSeriesCommand = new RoutedCommand("FixSeriesCommand", typeof(TransExternalCharWindow));
            _DeleteHiddenSeriesCommand = new RoutedCommand("DeleteHiddenSeriesCommand", typeof(TransExternalCharWindow));
        }
        #endregion

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
            var l = new Legend
            {
                LegendPlacement = LegendPlacement.Inside,
                LegendPosition = LegendPosition.RightTop,
                LegendBackground = OxyColor.FromAColor(200, OxyColors.White),
                LegendBorder = OxyColors.Black
            };
            pm.Legends.Add(l);
            return pm;
        }
        #region обработчики команд и событий
        async public void CalcExtChar(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            await Task.Run(() => ThisViewModel.CalcExtChar());                  // посчитать график во ViewModel
            
            if (ThisViewModel.TransExtChar.Count == 0)                          // если введены некорректные данные и график содержит 0 точек
            {                                                                   // то удалить график из плоттера (чтобы в легенде не отображалось несуществующего графика)
                Plotter.Series.Remove(EditedSeries);
                PlotView.InvalidatePlot(false);                                 // обновить плоттер
                return;
            }
            
            await UpdateExtCharSeriesAsync();                                   // обновить точки редактируемой серии

            if (!Plotter.Series.Contains(EditedSeries))                         // если плоттер не содержит редактируемый график,
                Plotter.Series.Add(EditedSeries);                               // то добавить его

            IAxis.MinimumRange = ThisViewModel.I2_step;      // нельзя приблизить больше, чем на шаг тока (избегаем подтормаживания)
            UAxis.MinimumRange = ThisViewModel.U1 / 100000;  // нельзя приблизить больше, чем в сто тысяч раз раз (избегаем подтормаживания)
                                                             // про MaximumRange сложно что-то сказать

            PlotView.ResetAllAxes();
            PlotView.InvalidatePlot();
        }
        private Task UpdateExtCharSeriesAsync()
        {
            return Task.Run(() => {
                EditedSeries.Points.Clear();
                foreach (var point in ThisViewModel.TransExtChar)
                {
                    EditedSeries.Points.Add(new DataPoint(point.Current, point.Voltage));
                }
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
        public void FixSeries_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if(!TryGetTitle($"Расчетная характеристика {CalcSeriesCounter}", out var title))
                return;

            CalcSeriesCounter++;

            LineSeries newSeries = new LineSeries                                           // создать новый график
            {
                Title = title,
                TrackerFormatString = "{1}: {2:.###}\n{3}: {4:.###}"
            };
            Plotter.Series.Remove(EditedSeries);                                            // удалить редактируемый график из плоттера
            Plotter.Series.Add(newSeries);                                                  // добавить новый график в плоттер
            newSeries.Points.AddRange(EditedSeries.Points);                                 // добавиь точки в новый график
            EditedSeries.Points.Clear();                                                    // очистить редактируемый график, чтобы отключить команду
            PlotView.InvalidatePlot(false);                                                 // обновить
        }
        private void FixSeries_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditedSeries.Points.Count != 0 && EditedSeries.IsVisible == true;
        }
        private void RefreshAxis(object sender, RoutedEventArgs e)
        {
            PlotView.ResetAllAxes();
            PlotView.InvalidatePlot(false);
        }
        private void DeleteHiddenSeries_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Plotter.Series.Any(series => series.IsVisible == false);
        }
        #endregion
        private void DeleteHiddenSeries_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Series[] deletedSeries = Plotter.Series.Where(series => series.IsVisible == false).ToArray();
            foreach (var series in deletedSeries)
            {
                Plotter.Series.Remove(series);
            }

            if (deletedSeries.Contains(EditedSeries))
            {
                EditedSeries.IsVisible = true;
                EditedSeries.Points.Clear();
            }


            PlotView.InvalidatePlot();
        }
        private bool TryGetTitle(string template, out string title)
        {
            EnterTitleWindow dialog = new EnterTitleWindow(template);
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                title = dialog.Text;
                return true;
            }
            else
            {
                title = null;
                return false;
            }
        }
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