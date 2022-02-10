using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TransformExtChar.Infrastructure;
using TransformExtChar.Infrastructure.Command;
using TransformExtChar.Model;

namespace TransformExtChar.ViewModel
{
    internal class PlotterVM : OnPropertyChangedClass
    {
        #region Команды

        #region Зафиксировать график
        public ICommand FixSeriesCommand { get; }
        public void FixSeries_Executed(object p)
        {
            MessageBus.Send(MessageEnum.AddFixedSeries_Open);
        }
        private bool FixSeries_CanExecuted(object p)
        {
            return EditedSeries.Points.Count != 0 && EditedSeries.IsVisible == true;
        }
        #endregion

        #region Удалить скрытые графики
        public ICommand DeleteHiddenSeriesCommand { get; }
        private void DeleteHiddenSeries_Execute(object p)
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

            MessageBus.Send(MessageEnum.InvalidatePlot_UpdateDataFalse);
        }
        private bool DeleteHiddenSeries_CanExecute(object p)
        {
            return Plotter.Series.Any(series => series.IsVisible == false);
        }
        #endregion

        #endregion

        #region Свойства и поля

        public List<VCData> TransExtChar { get; set; }

        private PlotModel _plotter;
        public PlotModel Plotter { get => _plotter; set => Set(ref _plotter, value); }
        private LineSeries EditedSeries { get; set; } // График, который будет изменяться при нажатии на "Построить..."
        private LinearAxis UAxis { get; }
        private LinearAxis IAxis { get; }

        #endregion

        public PlotterVM()
        {
            Plotter = CreatePlotModel();
            UAxis = (LinearAxis)Plotter.Axes[0];
            IAxis = (LinearAxis)Plotter.Axes[1];
            EditedSeries = new LineSeries
            {
                Title = "Рассчетная характеристика",                        // отличитильные черты редактируемого графика - название и цвет
                Color = OxyColors.Black,
                TrackerFormatString = "{1}: {2:.###}\n{3}: {4:.###}"
            };
            FixSeriesCommand = new RelayCommand(FixSeries_Executed, FixSeries_CanExecuted);
            DeleteHiddenSeriesCommand = new RelayCommand(DeleteHiddenSeries_Execute, DeleteHiddenSeries_CanExecute);
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
        async public void UpdatePlotter() 
        {
            if (TransExtChar.Count == 0)                                             // если введены некорректные данные и график содержит 0 точек
            {                                                                        // то удалить график из плоттера, если он там есть (чтобы в легенде    
                Plotter.Series.Remove(EditedSeries);                                 // не отображалось несуществующего графика)
                MessageBus.Send(MessageEnum.InvalidatePlot_UpdateDataFalse);         // обновить плоттер                          
                return;                                                              
            }                                                                        
                                                                                     
            await UpdateExtCharSeriesAsync();                                        // обновить точки редактируемой серии (EqivalentCircit возвращает список,
                                                                                     // который нельзя просто вставить в серию плоттера из-за неподходящего типа)
            if (!Plotter.Series.Contains(EditedSeries))                              // если плоттер не содержит редактируемый график,
                Plotter.Series.Add(EditedSeries);                                    // то добавить его
                                                                                     
            IAxis.MinimumRange = TransExtChar.Count > 1                              // нельзя приблизить больше, чем на шаг тока (избегаем подтормаживания)
                                 ? TransExtChar[1].Current - TransExtChar[0].Current
                                 : 1;                                       
            UAxis.MinimumRange = TransExtChar[0].Voltage / 100000;                   // нельзя приблизить больше, чем в сто тысяч раз раз (избегаем подтормаживания)
                                                                                     // про MaximumRange сложно что-то сказать

            MessageBus.Send(MessageEnum.UpdatePlotter_UpdateDataTrue);
        }
        private Task UpdateExtCharSeriesAsync()
        {
            return Task.Run(() => {
                EditedSeries.Points.Clear();
                foreach (var point in TransExtChar)
                {
                    EditedSeries.Points.Add(new DataPoint(point.Current, point.Voltage));
                }
            });
        }
        public void FixSeries(string title)
        {
            LineSeries newSeries = new LineSeries                                           // создать новый график
            {
                Title = title,
                TrackerFormatString = "{1}: {2:.###}\n{3}: {4:.###}"
            };

            Plotter.Series.Remove(EditedSeries);                                            // удалить редактируемый график из плоттера
            Plotter.Series.Add(newSeries);                                                  // добавить новый график в плоттер
            newSeries.Points.AddRange(EditedSeries.Points);                                 // добавиь точки в новый график
            EditedSeries.Points.Clear();                                                    // очистить редактируемый график, чтобы отключить команду
            MessageBus.Send(MessageEnum.InvalidatePlot_UpdateDataFalse);
        }
    }
}
