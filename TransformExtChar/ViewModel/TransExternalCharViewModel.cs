using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TransformExtChar.Model;
using TransformExtChar.Views;

namespace TransformExtChar.ViewModel
{
    internal class TransExternalCharViewModel : ViewModel
    {
        #region Команды
        private static RoutedCommand _CalcExtCharCommand;
        public static RoutedCommand CalcExtCharCommand { get { return _CalcExtCharCommand; } }

        private static RoutedCommand _CalcParamFromDataSheetCommand;
        public static RoutedCommand CalcParamFromDataSheetCommand { get { return _CalcParamFromDataSheetCommand; } }
        #endregion

        static TransExternalCharViewModel()
        {
            _CalcExtCharCommand = new RoutedCommand("CalcExtChar", typeof(TransExternalCharViewModel));
            _CalcParamFromDataSheetCommand = new RoutedCommand("CalcParamFromDataSheet", typeof(TransExternalCharViewModel));
        }

        #region Параметры
        private double _R1 = 0.67;
        public double R1 { get => _R1; set => Set(ref _R1, value); }

        private double _X1 = 0.11;
        public double X1 { get => _X1; set => Set(ref _X1, value); }

        private double _R2 = 0.67;
        public double R2 { get => _R2; set => Set(ref _R2, value); }

        private double _X2 = 0.11;
        public double X2 { get => _X2; set => Set(ref _X2, value); }

        private double _Rm = 45.013;
        public double Rm { get => _Rm; set => Set(ref _Rm, value); }

        private double _Xm = 285.95;
        public double Xm { get => _Xm; set => Set(ref _Xm, value); }

        private double _K = 1.91;
        public double K { get => _K; set => Set(ref _K, value); }

        private double _U1 = 220;
        public double U1 { get => _U1; set => Set(ref _U1, value); }

        private double _Fi2;
        public double Fi2 { get => _Fi2; set => Set(ref _Fi2, value); }

        private double _I2_start;
        public double I2_start { get => _I2_start; set => Set(ref _I2_start, value); }

        private double _I2_end;
        public double I2_end { get => _I2_end; set => Set(ref _I2_end, value); }

        private double _I2_step = 0.1;
        public double I2_step { get => _I2_step; set => Set(ref _I2_step, value); }
        #endregion

        private List<VCData> _transExtChar;
        public List<VCData> TransExtChar { get => _transExtChar; set => Set(ref _transExtChar, value); }

        async public void CalcExtChar()
        {
            Transformer transformer = new Transformer()
            {
                K = this.K,
                Z1 = new Complex(R1, X1),
                Z2_Сorrected = new Complex(R2, X2),
                Zm = new Complex(Rm, Xm)
            };
            const double toRad = Math.PI / 180;
            TransExtChar = await transformer.GetExternalCharacteristicAsync(Fi2 * toRad, I2_start, I2_end, U1, I2_step);
        }

        internal void CalcParamFromDataSheet(CalcParamFromDataSheetViewModel dialogViewModel)
        {
            if (dialogViewModel.TryGetTransExternalCharWindowFields(out var fields))
            {
                R1 = fields.Z1.Real;
                X1 = fields.Z1.Imaginary;
                R2 = fields.Z2_Сorrected.Real;
                X2 = fields.Z2_Сorrected.Imaginary;
                Rm = fields.Zm.Real;
                Xm = fields.Zm.Imaginary;
                K = fields.K;
                U1 = fields.U1;
                I2_start = fields.I2_start;
                I2_end = fields.I2_end;
            }
            else
            {
                MessageBox.Show("Такого трансформатора не может быть!", "Ошибка расчета", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
