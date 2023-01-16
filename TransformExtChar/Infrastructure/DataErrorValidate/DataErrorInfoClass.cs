using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace TransformExtChar.Infrastructure.DataErrorVerify
{
    public abstract class DataErrorInfoClass : OnPropertyChangedClass, IDataErrorInfo
    {
        public DataRule RegisterRule(Expression<Func<bool>> rule, string errorText)
        {
            var dr = CreateDataRule();
            _dataValidator.AddRule(dr);

            return dr;

            DataRule CreateDataRule()
            {
                var rulesProperties = GetPropertiesFromRule();
                var thisOjectProperties = GetType().GetProperties();

                var linkedPropetriesNames = rulesProperties.Intersect(thisOjectProperties).Select(p => p.Name);

                return new DataRule(rule.Compile(), errorText, linkedPropetriesNames);

                HashSet<PropertyInfo> GetPropertiesFromRule()
                {
                    var properties = new HashSet<PropertyInfo>();

                    GetPropertiesFromExpression(rule.Body, properties);

                    return properties;

                    void GetPropertiesFromExpression(Expression exp, HashSet<PropertyInfo> properties)
                    {
                        if (exp is MemberExpression memberExp && memberExp.Member is PropertyInfo property)
                            properties.Add(property);

                        else if (exp is BinaryExpression binExp)
                        {
                            GetPropertiesFromExpression(binExp.Left, properties);
                            GetPropertiesFromExpression(binExp.Right, properties);
                        }

                        else if (exp is UnaryExpression unaryExp)
                            GetPropertiesFromExpression(unaryExp.Operand, properties);

                        else if (exp is MethodCallExpression methodExp)
                            foreach (var arg in methodExp.Arguments)
                                GetPropertiesFromExpression(arg, properties);
                    }
                }
            }
        }
        public DataRule RegisterRule(Func<bool> rule, string errorText, IEnumerable<string> linkedPropertyNames)
        {
            var thisObjectPropertyNames = GetType().GetProperties().Select(p => p.Name);

            linkedPropertyNames = linkedPropertyNames.Intersect(thisObjectPropertyNames);

            var dr = new DataRule(rule, errorText, linkedPropertyNames);

            _dataValidator.AddRule(dr);

            return dr;
        }
        public DataRule RegisterRule_PropMustBeAboveOrEqualZero<T>(Expression<Func<T>> propertyLambda)
        {
            if (propertyLambda.Body is MemberExpression memberExp && memberExp.Member is PropertyInfo property)
            {
                var value = propertyLambda.Compile();
                Func<bool> rule = () => Comparer<T>.Default.Compare(value(), default(T)) >= 0;

                return RegisterRule_PropMustBeAboveOrEqualZero(rule, new string[] { property.Name });
            }
            else throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
        }
        public DataRule RegisterRule_PropMustBeAboveOrEqualZero<T>(Expression<Func<T>> memberLambda, IEnumerable<string> propertyNames)
        {
            if (memberLambda.Body is MemberExpression memberExp)
            {
                var value = memberLambda.Compile();
                Func<bool> rule = () => Comparer<T>.Default.Compare(value(), default(T)) >= 0;

                return RegisterRule_PropMustBeAboveOrEqualZero(rule, propertyNames);
            }
            else throw new ArgumentException("You must pass a lambda of the form: '() => Class.Member' or '() => object.Member'");
        }
        private DataRule RegisterRule_PropMustBeAboveOrEqualZero(Func<bool> propertyLambda, IEnumerable<string> propertyNames)
        {
            return RegisterRule(propertyLambda, "Не может быть меньше нуля", propertyNames);
        }
        public DataRule RegisterRule_PropMustBeAboveZero<T>(Expression<Func<T>> propertyLambda)
        {
            if (propertyLambda.Body is MemberExpression memberExp && memberExp.Member is PropertyInfo property)
            {
                var value = propertyLambda.Compile();
                Func<bool> rule = () => Comparer<T>.Default.Compare(value(), default(T)) > 0;

                return RegisterRule_PropMustBeAboveZero(rule, new string[] { property.Name });
            }
            else throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
        }
        public DataRule RegisterRule_PropMustBeAboveZero<T>(Expression<Func<T>> memberLambda, IEnumerable<string> propertyNames)
        {
            if(memberLambda.Body is MemberExpression memberExp)
            {
                var value = memberLambda.Compile();
                Func<bool> rule = () => Comparer<T>.Default.Compare(value(), default(T)) > 0;

                return RegisterRule_PropMustBeAboveZero(rule, propertyNames);
            }
            else throw new ArgumentException("You must pass a lambda of the form: '() => Class.Member' or '() => object.Member'");
        }
        private DataRule RegisterRule_PropMustBeAboveZero(Func<bool> rule, IEnumerable<string> propertyNames)
        {
            return RegisterRule(rule, "Не может быть меньше или равно нулю", propertyNames);
        }
        protected override void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            _dataValidator.ValidateData(PropertyName);

            var linkedPropertyNames = _dataValidator.GetPropertyNamesLinkedWith(PropertyName);

            foreach (var linkedPropertyName in linkedPropertyNames)
                base.OnPropertyChanged(linkedPropertyName);

            if (!linkedPropertyNames.Contains(PropertyName))
                base.OnPropertyChanged(PropertyName);
        }
        public void CheckRuleAndSetError(DataRule dataRule)
        {
            _dataValidator.CheckdRuleAndSetError(dataRule);
        }
        public DataErrorInfoClass()
        {
            _dataValidator = new DataErrorValidator(errors);
        }

        private DataErrorValidator _dataValidator;

        protected void CheckAllRulesAndSetError()
        {
            _dataValidator.CheckAllRulesAndSetError();
        }

        #region Реализация IDataErrorInfo
        [JsonIgnore]
        public string Error => errors.Any(str => str.Value != null) ? "Error" : string.Empty;
        public string this[string columnName] => errors.ContainsKey(columnName) ? errors[columnName] : null;
        private Dictionary<string, string> errors = new Dictionary<string, string>();
        #endregion
    }
}
