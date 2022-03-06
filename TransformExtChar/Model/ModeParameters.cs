using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TransformExtChar.Infrastructure;

namespace TransformExtChar.Model
{
    public class ModeParameters : OnPropertyChangedClass, IDataErrorInfo
    {
        #region Проверки ошибок данных
        private void ResistanceAngleCheck([CallerMemberName] string PropertyName = null)
        {
            DataErrorChecker.CheckErrors(() => Fi2 > 90 || Fi2 < -90, "Угол сопротивления должен находиться в пределах ±90 градусов", errors, PropertyName);
        }
        #endregion
        
        #region Параметры
        private double _U1 = 220;
        public double U1 
        { 
            get => _U1;
            set
            {
                if(Set(ref _U1, value)) DataErrorChecker.AboveZeroCheck(value, errors);
            }
        }
        
        private double _Fi2;
        public double Fi2
        {
            get => _Fi2;
            set
            {
                if (Set(ref _Fi2, value)) ResistanceAngleCheck();
            }
        }

        private double _I2_start;
        public double I2_start
        {
            get => _I2_start;
            set
            {
                if (Set(ref _I2_start, value)) DataErrorChecker.AboveOrEqualZeroCheck(value, errors);
            }
        }

        private double _I2_end;
        public double I2_end
        {
            get => _I2_end;
            set
            {
                if (Set(ref _I2_end, value)) DataErrorChecker.AboveOrEqualZeroCheck(value, errors);
            }
        }

        private double _I2_step = 0.1;
        public double I2_step 
        {
            get => _I2_step;
            set
            {
                if(Set(ref _I2_step, value)) DataErrorChecker.AboveZeroCheck(value, errors);
            }
        }
        #endregion

        #region Реализация IDataErrorInfo
        public string Error  => errors.Any(str => str.Value != null) ? "Error" : string.Empty;

        public string this[string columnName] => errors.ContainsKey(columnName) ? errors[columnName] : null;

        private Dictionary<string, string> errors = new Dictionary<string, string>();
        #endregion
    }
}
