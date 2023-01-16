using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TransformExtChar.Infrastructure
{
    public class DataRule
    {
        private Func<bool> _rule;
        public string ErrorText { get; }
        public List<string> LinkedPropertyNames { get; }

        public event RuleValidateEventHandler Validate;

        public bool ValidateRule()
        {
            bool result = _rule();

            Validate?.Invoke(this, new RuleValidateEventArgs(this, result));

            return result;
        }

        public DataRule(Func<bool> rule, string errorText, IEnumerable<string> linkedPropetryNames) =>
            (_rule, ErrorText, LinkedPropertyNames) = (rule, errorText, linkedPropetryNames.ToList());
    }

    public delegate void RuleValidateEventHandler(object sender, RuleValidateEventArgs e);

    public class RuleValidateEventArgs : EventArgs
    {
        public DataRule ValidatedRule { get; }
        public bool RuleResult { get; }

        public RuleValidateEventArgs(DataRule dataRule, bool ruleResult) => (ValidatedRule, RuleResult) = (dataRule, ruleResult);
    }
}
