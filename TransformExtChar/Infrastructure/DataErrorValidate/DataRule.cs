using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TransformExtChar.Infrastructure
{
    public class DataRule
    {
        public Func<bool> Rule { get; }
        public string ErrorText { get; }
        public List<string> LinkedPropertyNames { get; }

        public event RuleValidateEventHandler Validate;

        public bool ValidateRule()
        {
            bool result = Rule();

            Validate?.Invoke(this, new RuleValidateEventArgs(this, result));

            return result;
        }

        public DataRule(Func<bool> rule, string errorText, IEnumerable<string> linkedPropetryNames) =>
            (Rule, ErrorText, LinkedPropertyNames) = (rule, errorText, linkedPropetryNames.ToList());
    }

    public delegate void RuleValidateEventHandler(object sender, RuleValidateEventArgs e);

    public class RuleValidateEventArgs : EventArgs
    {
        public DataRule ValidatedRule { get; }
        public bool RuleResult { get; }

        public RuleValidateEventArgs(DataRule dataRule, bool ruleResult) => (ValidatedRule, RuleResult) = (dataRule, ruleResult);
    }
}
