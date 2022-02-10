using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using TransformExtChar.Infrastructure;
using TransformExtChar.Infrastructure.Command;
using TransformExtChar.Model;

namespace TransformExtChar.ViewModel
{
    internal class CalcParamFromDataSheetViewModel : OnPropertyChangedClass
    {
        #region Команды

        #region Ok
        public ICommand OkCommand { get; }

        private void Ok_Execute(object p)
        {
            MessageBus.Send(MessageEnum.CalcParamFromDataSheet_Close);

            if (!TrySetMainWindowFields())
                MessageBox.Show("Такого трансформатора не может быть!", "Ошибка расчета", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public bool TrySetMainWindowFields()
        {
            if (TryCalcParam(out var param))
            {
                UpdatePreview();

                EquivalentCurcuitVM.Rm = param.Zm.Real;
                EquivalentCurcuitVM.Xm = param.Zm.Imaginary;
                EquivalentCurcuitVM.R1 = param.Z1.Real;
                EquivalentCurcuitVM.X1 = param.Z1.Imaginary;
                EquivalentCurcuitVM.R2 = param.Z2_Сorrected.Real;
                EquivalentCurcuitVM.X2 = param.Z2_Сorrected.Imaginary;
                EquivalentCurcuitVM.K = param.K;
                ModeParametersVM.U1 = U1r;
                ModeParametersVM.I2_start = 0;
                ModeParametersVM.I2_end = 1.1 * I1r;  // выражение будет разное в зависимости от того, нужен приведенный ток вторичной обмотки или 
                                          // реальный. Здесь посчитано 110% от номинального приведённого тока вторичной обмотки
                return true;
            }
            else
            {
                return false;
            }
        }
        internal bool TryCalcParam(out (Complex Zm, Complex Z1, Complex Z2_Сorrected, double K) param)
        {
            var specifications = new TransformerDatasheetSpecifications 
            {
                U1_Rated = U1r,
                U2_Rated = U2r,
                I1_Rated = I1r,
                I1_Unload = I0,
                P_Unload = P0,
                U1_ShortCircuit = U1sc,
                P_ShortCircuit = Psc
            };
            return specifications.TryGetEquivalentCurcuitParameters(out param);
        }
        private void UpdatePreview()
        {
            PreviewDataSheet.U1r = U1r;
            PreviewDataSheet.U2r = U2r;
            PreviewDataSheet.I1r = I1r;
            PreviewDataSheet.I0 = I0;
            PreviewDataSheet.P0 = P0;
            PreviewDataSheet.U1sc = U1sc;
            PreviewDataSheet.Psc = Psc;
        }
        #endregion

        #region Cancel
        public ICommand CancleCommand { get; }

        private void Cancle_Execute(object p)
        {
            MessageBus.Send(MessageEnum.CalcParamFromDataSheet_Close);
        }
        #endregion
        
        #endregion

        public CalcParamFromDataSheetViewModel()
        {
            U1r = PreviewDataSheet.U1r;
            U2r = PreviewDataSheet.U2r;
            I1r = PreviewDataSheet.I1r;
            I0 = PreviewDataSheet.I0;
            P0 = PreviewDataSheet.P0;
            U1sc = PreviewDataSheet.U1sc;
            Psc = PreviewDataSheet.Psc;

            OkCommand = new RelayCommand(Ok_Execute);
            CancleCommand = new RelayCommand(Cancle_Execute);
        }

        #region Параметры
        private static (double U1r, double U2r, double I1r, double I0, double P0, double U1sc, double Psc) PreviewDataSheet = (220, 115, 7.3, 0.76, 26, 10, 72);

        private double _U1r;
        public double U1r { get => _U1r; set => Set(ref _U1r, value); }

        private double _U2r;
        public double U2r { get => _U2r; set => Set(ref _U2r, value); }

        private double _I1r;
        public double I1r { get => _I1r; set => Set(ref _I1r, value); }

        private double _I0;
        public double I0 { get => _I0; set => Set(ref _I0, value); }

        private double _P0;
        public double P0 { get => _P0; set => Set(ref _P0, value); }

        private double _U1sc;
        public double U1sc { get => _U1sc; set => Set(ref _U1sc, value); }

        private double _Psc;
        public double Psc { get => _Psc; set => Set(ref _Psc, value); }
        #endregion

        public EquivalentCurcuitViewModel EquivalentCurcuitVM { get; set; }
        public ModeParametersViewModel ModeParametersVM { get; set; }
    }
}
