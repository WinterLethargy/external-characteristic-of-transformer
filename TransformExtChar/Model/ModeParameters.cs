using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TransformExtChar.Infrastructure;
using TransformExtChar.Infrastructure.DataErrorVerify;

namespace TransformExtChar.Model
{
    public class ModeParameters : DataErrorInfoClass, IDataErrorInfo
    {
        #region Параметры
        private double _U1 = 220;
        public double U1 
        { 
            get => _U1;
            set => Set(ref _U1, value);
        }
        
        private double _Fi2;
        public double Fi2
        {
            get => _Fi2;
            set => Set(ref _Fi2, value);
        }

        private double _I2_start;
        public double I2_start
        {
            get => _I2_start;
            set => Set(ref _I2_start, value);
        }

        private double _I2_end;
        public double I2_end
        {
            get => _I2_end;
            set => Set(ref _I2_end, value);
        }

        private double _I2_step = 0.1;
        public double I2_step 
        {
            get => _I2_step;
            set => Set(ref _I2_step, value);
        }
        #endregion
        public ModeParameters()
        {
            RegisterRule_PropMustBeAboveZero(() => U1);
            RegisterRule(() => Fi2 <= 90 && Fi2 >= -90, "Угол сопротивления должен находиться в пределах ±90 градусов");
            RegisterRule_PropMustBeAboveOrEqualZero(() => I2_start);
            RegisterRule_PropMustBeAboveOrEqualZero(() => I2_end);
            RegisterRule_PropMustBeAboveZero(() => I2_step);
        }
    }
}
