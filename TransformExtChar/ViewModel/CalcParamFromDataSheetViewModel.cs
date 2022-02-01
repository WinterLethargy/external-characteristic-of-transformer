using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Windows.Input;
using TransformExtChar.Model;

namespace TransformExtChar.ViewModel
{
    class CalcParamFromDataSheetViewModel : ViewModel
    {
        #region Команды
        private static RoutedCommand _CalcParam;
        public static RoutedCommand CalcParamCommand { get { return _CalcParam; } }

        private static RoutedCommand _Cancle;
        public static RoutedCommand CancleCommand { get { return _Cancle; } }
        #endregion

        static CalcParamFromDataSheetViewModel()
        {
            _CalcParam = new RoutedCommand("CalcParam", typeof(TransExternalCharViewModel));
            _Cancle = new RoutedCommand("Cancle", typeof(TransExternalCharViewModel));
        }

        public CalcParamFromDataSheetViewModel()
        {
            U1r = Cash.U1r;
            U2r = Cash.U2r;
            I1r = Cash.I1r;
            I0 = Cash.I0;
            P0 = Cash.P0;
            U1sc = Cash.U1sc;
            Psc = Cash.Psc;
        }

        #region Параметры
        private static (double U1r, double U2r, double I1r, double I0, double P0, double U1sc, double Psc) Cash = (220, 115, 7.3, 0.76, 26, 10, 72);

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

        internal bool TryCalcParam(out (Complex Zm, Complex Z1, Complex Z2_Сorrected, double K) param)
        {
            var specifications = new TransformerDatasheetSpecifications(U1r, U2r, I1r, I0, P0, U1sc, Psc);
            return specifications.TryGetTransformerParameters(out param);
        }

        private void UpdateCash()
        {
            Cash.U1r = U1r;
            Cash.U2r = U2r;
            Cash.I1r = I1r;
            Cash.I0 = I0;
            Cash.P0 = P0;
            Cash.U1sc = U1sc;
            Cash.Psc = Psc;
        }

        public bool TryGetTransExternalCharWindowFields(out (Complex Zm, Complex Z1, Complex Z2_Сorrected, double K, double U1, double I2_start, double I2_end) fields)
        {
            if(TryCalcParam(out var param))
            {
                UpdateCash();

                fields.Zm = param.Zm;
                fields.Z1 = param.Z1;
                fields.Z2_Сorrected = param.Z2_Сorrected;
                fields.K = param.K;
                fields.U1 = U1r;
                fields.I2_start = 0;
                fields.I2_end = 1.1 * I1r;// выражение будет разное в зависимости от того, нужен приведенный ток вторичной обмотки или 
                                          // реальный. Здесь посчитано 110% от номинального приведённого тока вторичной обмотки
                return true;
            }
            else
            {
                fields = (0, 0, 0, 0, 0, 0, 0);
                return false;
            }
        }
    }
}
