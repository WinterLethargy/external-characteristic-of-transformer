using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TransformExtChar.Infrastructure;
using TransformExtChar.Infrastructure.Command;
using TransformExtChar.Model;
using TransformExtChar.Services;
using TransformExtChar.Views;

namespace TransformExtChar.ViewModel
{
    internal class TransExternalCharViewModel : OnPropertyChangedClass
    {
        #region Команды

        #region Посчитать график внешней характерискики (CalcExtCharCommand)
        public ICommand CalcExtCharCommand { get; }

        async private void CalcExtChar_Executed(object p)
        { 
            var newCharacteristic = await Task.Run(() => CalcExtChar());
            PlotterVM.UpdateEditedSeries(newCharacteristic);   // посчитать график
        }

        private List<VCPointData> CalcExtChar()
        {
            var equivalentCurcuit = EquivalentCurcuitVM.GetEquivalentCurcuit();
            const double toRad = Math.PI / 180;
            return equivalentCurcuit.GetExternalCharacteristic(Fi2 * toRad, I2_start, I2_end, U1, I2_step);
        }
        #endregion

        #region Посчитать параметры схемы замещения из паспортных данных трансформатора (CalcParamFromDataSheetCommand)
        public ICommand CalcParamFromDataSheetCommand { get; }
        public void CalcParamFromDataSheet_Executed(object p)
        {
            MessageBus.Send(MessageEnum.CalcParamFromDataSheet_Open);
        }
        #endregion

        #endregion

        #region Параметры схемы замещения и её расчёта
        public EquivalentCurcuitViewModel EquivalentCurcuitVM { get; set; } = new EquivalentCurcuitViewModel(); // VM здесь нужна, потому что !!!!!!!!!!!!!!!
        public ModeParametersViewModel ModeParametersVM { get; set; } = new ModeParametersViewModel();
        public double U1 { get => ModeParametersVM.U1; }
        public double Fi2 { get => ModeParametersVM.Fi2; }
        public double I2_start { get => ModeParametersVM.I2_start; }
        public double I2_end { get => ModeParametersVM.I2_end; }
        public double I2_step { get => ModeParametersVM.I2_step; }
        #endregion

        #region Свойства для работы плоттера
        public PlotterVM PlotterVM { get; set; } = new PlotterVM();
        #endregion

        #region Конструкторы
        public TransExternalCharViewModel()
        {
            CalcExtCharCommand = new RelayCommand(CalcExtChar_Executed);
            CalcParamFromDataSheetCommand = new RelayCommand(CalcParamFromDataSheet_Executed);
        }
        #endregion
    }
}
