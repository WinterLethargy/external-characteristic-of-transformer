using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using TransformExtChar.Infrastructure;
using TransformExtChar.Infrastructure.Command;

namespace TransformExtChar.ViewModel
{
    internal class AddSeriesVM : OnPropertyChangedClass
    {
        #region Команды

        #region Ok
        public ICommand OkCommand { get; }
        private void Ok_Execute(object p) 
        {
            MessageBus.Send(MessageEnum.AddSeries_Close);

            PlotterVM.FixSeries(Title);

            CalcSeriesCounter++;
        }
        #endregion

        #region Cancel
        public ICommand CancleCommand { get; }
        private void Cancle_Execute(object p)
        {
            MessageBus.Send(MessageEnum.AddSeries_Close);
        }
        #endregion

        #endregion

        public AddSeriesVM()
        {
            Title = $"Расчетная характеристика {CalcSeriesCounter}";

            OkCommand = new RelayCommand(Ok_Execute);
            CancleCommand = new RelayCommand(Cancle_Execute);
        }

        private static int CalcSeriesCounter { get; set; } = 1;

        private string _title;
        public string Title { get => _title; set => Set(ref _title, value); }

        public PlotterVM PlotterVM { get; set; }
    }
}
