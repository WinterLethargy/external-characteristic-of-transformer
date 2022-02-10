using System;
using System.Collections.Generic;
using System.Text;
using TransformExtChar.Infrastructure;

namespace TransformExtChar.ViewModel
{
    class ModeParametersViewModel : OnPropertyChangedClass
    {
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
    }
}
