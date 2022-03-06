using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TransformExtChar.Model
{
    static class DataErrorChecker
    {
        static public void CheckErrors(Func<bool> predicate, string text, Dictionary<string, string> errors, string PropertyName)
        {
            errors.TryGetValue(PropertyName, out string error);

            string newLineText = "\n" + text;

            if (predicate())
                if (error == null)
                    errors[PropertyName] = text;
                else
                {
                    if (error.Contains(text))
                        return;
                    else
                        errors[PropertyName] = errors[PropertyName] + newLineText;
                }

            else
            {
                if (error == null)
                    return;

                int index = error.IndexOf(text);
                if (index == -1)
                    return;

                if (index != 0)
                {
                    errors[PropertyName] = error.Remove(index - "\n".Length, newLineText.Length);
                    return;
                }

                if (index == 0)
                {
                    errors[PropertyName] = error == text ?
                                           null :
                                           error.Remove(0, newLineText.Length);
                    return;
                }
            }
        }
        /// <summary>
        /// Ошибка данных, если value меньше либо равно нулю
        /// </summary>
        /// <param name="value"></param>
        /// <param name="errors"></param>
        /// <param name="text"></param>
        /// <param name="PropertyName"></param>
        static public void AboveZeroCheck(double value, Dictionary<string, string> errors, string text = "Не может быть меньше или равно нулю", [CallerMemberName] string PropertyName = null)
        {
            CheckErrors(() => value <= 0, text, errors, PropertyName);
        }
        /// <summary>
        /// Ошибка данных, если value меньше нуля
        /// </summary>
        /// <param name="value"></param>
        /// <param name="errors"></param>
        /// <param name="text"></param>
        /// <param name="PropertyName"></param>
        static public void AboveOrEqualZeroCheck(double value, Dictionary<string, string> errors, string text = "Не может быть меньше нуля", [CallerMemberName] string PropertyName = null)
        {
            CheckErrors(() => value < 0, text, errors, PropertyName);
        }
    }
}
