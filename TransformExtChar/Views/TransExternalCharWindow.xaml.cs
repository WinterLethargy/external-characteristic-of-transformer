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
using TransformExtChar.Infrastructure;
using TransformExtChar.Infrastructure.Converters;
using TransformExtChar.ViewModel;
using TransformExtChar.Views;

namespace TransformExtChar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class TransExternalCharWindow : Window
    {
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBus.Bus += Receive;
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            MessageBus.Bus -= Receive;
        }
        public TransExternalCharWindow()
        {
            InitializeComponent();
        }
        private void Receive(object message, object p)
        {
            if (!(message is MessageEnum mes))
            {
                return;
            }
            else
            {
                switch (mes)
                {
                    case MessageEnum.UpdatePlotter_UpdateDataFalse: UpdatePlotter(true, false); break;
                    case MessageEnum.UpdatePlotter_UpdateDataTrue: UpdatePlotter(true, true); break;
                    case MessageEnum.InvalidatePlot_UpdateDataFalse:  UpdatePlotter(false, false); break;
                    case MessageEnum.InvalidatePlot_UpdateDataTrue: UpdatePlotter(false, true); break;
                    case MessageEnum.AddSeries_Open: OpenAddSeriesWindow(mes, p); break;
                    default: break;
                }
            }
        }
        private void RefreshAxis(object sender, RoutedEventArgs e)
        {
            UpdatePlotter(true, true);
        }
        private void UpdatePlotter(bool resetAxes, bool updateData)
        {
            if (resetAxes) PlotView.ResetAllAxes();
            PlotView.InvalidatePlot(updateData);
        }
        private void OpenAddSeriesWindow(MessageEnum fixedOrUsers, object p)
        {
            var dialog = new AddSeriesWindow() { Owner = this };

            var dialogVM = (AddSeriesViewModel)dialog.DataContext;
            var thisVM = (TransExternalCharViewModel)DataContext;        // нельзя в конструкторе, потому что Owner на тот момент ещё не инициализировано

            dialogVM.PlotterVM = thisVM.PlotterVM;

            dialog.ShowDialog();
        }

        private void Plotter_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data as DataObject;
            if (data != null && data.ContainsFileDropList())
            {
                foreach (var file in data.GetFileDropList())
                {
                    //if(AddSeriesFromFileMenuItem.Command.CanExecute(new object()))    // сразу после запуска команда ещё не привязана
                    //    AddSeriesFromFileMenuItem.Command.Execute(file);
                    var plotterVM = ((TransExternalCharViewModel)DataContext).PlotterVM;

                    if (plotterVM.AddSeriesFromFileCommand.CanExecute(null))
                        plotterVM.AddSeriesFromFileCommand.Execute(file);
                }
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