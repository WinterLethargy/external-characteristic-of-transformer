using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TransformExtChar.Infrastructure;
using TransformExtChar.Infrastructure.Command;
using TransformExtChar.Model;
using TransformExtChar.Services;

namespace TransformExtChar.ViewModel
{
    internal class PlotterViewModel : OnPropertyChangedClass
    {
        #region Команды

        #region Зафиксировать график
        public ICommand FixSeriesCommand { get; }
        public void FixSeries_Executed(object p)
        {
            MessageBus.Send(MessageEnum.AddSeries_Open);
        }
        private bool FixSeries_CanExecuted(object p)
        {
            return EditedSeries.Points.Count != 0 && EditedSeries.IsVisible == true;
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

        #region Добавить график из файла
        public ICommand AddSeriesFromFileCommand { get; }
        private bool IsAddSeriesFromFileExecuting;
        private bool AddSeriesFromFile_CanExecute(object p) => !IsAddSeriesFromFileExecuting;
        private void AddSeriesFromFile_Execute(object p)
        {
            if (p is string path)
                Task.Run(() => AddSeriesFromFile(path));
            else
                Task.Run(() => AddSeriesFromFile());
        }
        private void AddSeriesFromFile(string path = null)
        {
            IsAddSeriesFromFileExecuting = true;

            if (path != null && !Path.IsPathFullyQualified(path))   // если не нулл и полный путь, тогда без изменений
                path = null;

            if (DataService.TryOpenCSV(out var csvDocument, path))                              // метод учитывает path == null. тогда открывается OpenFoleDialog 
                if (csvDocument.Headers[0].Equals("I", StringComparison.OrdinalIgnoreCase))
                    foreach (var series in csvDocument.GetData())
                        AddSeries(series.title, series.points);
                else
                {
                    MessageBox.Show("Недопустимое форматирование!\nПервая колонка должна быть \"I\".", "Недопустимое форматирование!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            IsAddSeriesFromFileExecuting = false;
        }
        public void AddSeries(string title, List<Point> points)
        {
            if (title.Length > 40)
            {
                title = title.Substring(0, 37) + "...";
            }
            LineSeries newSeries = new LineSeries
            {
                Title = title,
                TrackerFormatString = "{1}: {2:.###}\n{3}: {4:.###}",
                MarkerType = MarkerType.Circle,
                MarkerSize = 2.5
            };

            foreach (var point in points)
            {
                newSeries.Points.Add(new DataPoint(point.X, point.Y));
            }

            Plotter.Series.Add(newSeries);

            MessageBus.Send(MessageEnum.UpdatePlotter_UpdateDataTrue);
        }
        #endregion

        #region сохранить PNG
        public ICommand SavePNGCommand { get; }
        private void SavePNG_Execute(object p)
        {
            if (!(p is PlotView plotView))
                return;

            var dlg = new SaveFileDialog
            {
                Filter = ".png files|*.png",
                DefaultExt = ".png"
            };
            if (dlg.ShowDialog().Value)                                                     // Чтобы вызвать PngExporter.Export() без обращения к диспетчеру
            {                                                                               // код должен выполняться в потоке пользовательского интерфейса
                PngExporter.Export(Plotter, dlg.FileName, 5 * (int)plotView.ActualWidth, 5 * (int)plotView.ActualHeight, 500); 
            }
        }
        private bool SavePNG_CanExecute(object p)
        {
            return Plotter.Series.Any(series => series.IsVisible == true);
        }
        #endregion

        #endregion

        #region Свойства и поля

        private PlotModel _plotter;
        public PlotModel Plotter { get => _plotter; set => Set(ref _plotter, value); }
        private LineSeries EditedSeries { get; set; } // График, который будет изменяться при нажатии на "Построить..."

        #endregion

        private DataService DataService { get; } = new DataService();

        public PlotterViewModel()
        {
            Plotter = CreatePlotModel();
            EditedSeries = new LineSeries
            {
                Title = "Рассчетная характеристика",                        // отличитильные черты редактируемого графика - название и цвет
                Color = OxyColors.Black,
                TrackerFormatString = "{1}: {2:.###}\n{3}: {4:.###}"
            };
            FixSeriesCommand = new RelayCommand(FixSeries_Executed, FixSeries_CanExecuted, "Зафиксировать график");
            DeleteHiddenSeriesCommand = new RelayCommand(DeleteHiddenSeries_Execute, DeleteHiddenSeries_CanExecute, "Удалить скрытые графики\n(чтобы скрыть график, кликните по его имени в легенде)");
            AddSeriesFromFileCommand = new RelayCommand(AddSeriesFromFile_Execute, AddSeriesFromFile_CanExecute, "Загрузить графики из CSV файла");
            SavePNGCommand = new RelayCommand(SavePNG_Execute, SavePNG_CanExecute, "Сохранить плоттер в PNG");
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
                LegendBorder = OxyColors.Black,
                LegendFontSize = 10
            };
            pm.Legends.Add(l);
            return pm;
        }
        async internal void UpdateEditedSeries(List<VCPointData> newSeries)
        {
            if (newSeries.Count == 0)                                                // если введены некорректные данные и график содержит 0 точек
            {                                                                        // то удалить график из плоттера, если он там есть (чтобы в легенде    
                Plotter.Series.Remove(EditedSeries);                                 // не отображалось несуществующего графика)
                MessageBus.Send(MessageEnum.InvalidatePlot_UpdateDataFalse);         // обновить плоттер                          
                return;
            }

            await UpdateEditedSeriesPointsAsync(newSeries);                          // обновить точки редактируемой серии (EqivalentCircit возвращает список,
                                                                                     // который нельзя просто вставить в серию плоттера из-за неподходящего типа)
            if (!Plotter.Series.Contains(EditedSeries))                              // если плоттер не содержит редактируемый график,
                Plotter.Series.Add(EditedSeries);                                    // то добавить его

            MessageBus.Send(MessageEnum.UpdatePlotter_UpdateDataTrue);


            Application.Current.Dispatcher.BeginInvoke((Action) (() => CommandManager.InvalidateRequerySuggested()));// это не должно вызываться здесь, здесь должно быть сообщение шине сообщений
                                                                                                                     // но где должен выполняться обработчик этого сообщения?
                                                                                                                     // это странно, но почему-то этот вызов обновляет состояние команд только при выполнении
                                                                                                                     // в этом методе, если его вызвать в вызывающей функции, то обновления не произойдёт (после первой компиляции
                                                                                                                     // и запуске обновление команд происходит, при дальнейших компиляциях не происходит.. это как вообще?)


        }
        private Task UpdateEditedSeriesPointsAsync(List<VCPointData> newCharacteristic)
        {
            return Task.Run(() => {
                EditedSeries.Points.Clear();
                foreach (var point in newCharacteristic)
                {
                    EditedSeries.Points.Add(new DataPoint(point.Current, point.Voltage));
                }
            });
        }
    }
}
