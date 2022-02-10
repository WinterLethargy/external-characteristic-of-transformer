using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TransformExtChar.Infrastructure;
using TransformExtChar.Model;

namespace TransformExtChar.ViewModel
{
    internal class EquivalentCurcuitViewModel : OnPropertyChangedClass
    {
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

        public EquivalentCurcuit GetEquivalentCurcuit()
        {
            return new EquivalentCurcuit()
            {
                K = K,
                Z1 = new Complex(R1, X1),
                Z2_Сorrected = new Complex(R2, X2),
                Zm = new Complex(Rm, Xm)
            };
        }
    }
}
