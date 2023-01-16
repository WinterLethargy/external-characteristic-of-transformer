using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace TransformExtChar.Infrastructure
{
    public class DataErrorValidator
    {
        public void ValidateData(string propertyName)
        {
            if (propertyDataRules.TryGetValue(propertyName, out var dataRules))
                foreach (var dataRule in dataRules)
                    CheckRegisteredRuleAndSetError(dataRule);
        }
        public void CheckAllRulesAndSetError()
        {
            var ruleLists = propertyDataRules.Values;
            var rulesSet = new HashSet<DataRule>();

            foreach (var ruleList in ruleLists)
                ruleList.ForEach(r => rulesSet.Add(r));

            foreach (var rule in rulesSet)
                CheckRegisteredRuleAndSetError(rule);
        }
        public void CheckdRuleAndSetError(DataRule dataRule)
        {
            if (IsRegisteredRule(dataRule))
                CheckRegisteredRuleAndSetError(dataRule);
            else throw new ArgumentException("Это правило не содержится в зарегистрированных правилах");
        }

        private HashSet<DataRule> _chekingRules = new HashSet<DataRule>();
        private void CheckRegisteredRuleAndSetError(DataRule dataRule)
        {
            if (_chekingRules.Contains(dataRule))
                return;

            _chekingRules.Add(dataRule);

            var errorText = dataRule.ErrorText;
            string newLineErrorText = "\n" + errorText;

            if (dataRule.ValidateRule())
                dataRule.LinkedPropertyNames.ForEach(pn => RemoveError(pn));
            else
                dataRule.LinkedPropertyNames.ForEach(pn => SetError(pn));
            
            _chekingRules.Remove(dataRule);

            void SetError(string propertyName)
                {
                    errors.TryGetValue(propertyName, out string oldErrorText);

                    if (oldErrorText == null || oldErrorText == string.Empty)
                    {
                        errors[propertyName] = errorText;
                    }
                    else
                    {
                        if (oldErrorText.Contains(errorText))
                            return;
                        else
                            errors[propertyName] = errors[propertyName] + newLineErrorText;
                    }
                }

            void RemoveError(string propertyName)
                {
                    errors.TryGetValue(propertyName, out string oldErrorText);

                    if (oldErrorText == null)
                    {
                        return;
                    }

                    else if(oldErrorText.StartsWith(errorText))
                    {
                        errors[propertyName] = errors[propertyName].Replace(errorText + '\n', "");
                        errors[propertyName] = errors[propertyName].Replace(errorText, "");

                        if (errors[propertyName] == String.Empty)
                            errors[propertyName] = null;

                        return;
                    }
                    else
                    {
                        errors[propertyName] = errors[propertyName].Replace(newLineErrorText, ""); // в случае отсутствия соответствия со строчкой ничего не происходит
                        errors[propertyName] = errors[propertyName].Replace(errorText, "");        // следует переделать в регулярное выражение

                        var debug = errors[propertyName];
                    }
                }
        }
        public IEnumerable<string> GetPropertyNamesLinkedWith(string propertyName)
        {
            if(!propertyDataRules.TryGetValue(propertyName, out var dataRules))
                return Enumerable.Empty<string>();

            var result = new HashSet<string>();

            foreach (var dataRule in dataRules)
            {
                dataRule.LinkedPropertyNames.ForEach(pn => result.Add(pn));
            }

            return result;
        }
        public void AddRule(DataRule dataRule)
        {
            if (IsRegisteredRule(dataRule))
                return;

            foreach (var propertyName in dataRule.LinkedPropertyNames)
            {
                if (!propertyDataRules.ContainsKey(propertyName))
                    propertyDataRules[propertyName] = new List<DataRule>();

                propertyDataRules[propertyName].Add(dataRule);
            }
        }
        public DataErrorValidator(Dictionary<string, string> Errors)
        {
            errors = Errors;
            propertyDataRules = new Dictionary<string, List<DataRule>>();
        }

        private bool IsRegisteredRule(DataRule dataRule)
        {
            var ruleLists = propertyDataRules.Values;
            var containsDataRuleLists = ruleLists.Where(ruleList => ruleList.Contains(dataRule));
            return containsDataRuleLists.Any();
        }
        private Dictionary<string, List<DataRule>> propertyDataRules;
        private Dictionary<string, string> errors;
    }
}
