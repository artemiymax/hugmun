using System;

namespace HugMun.Case.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LabelCaseSolutionAttribute : CaseAttributeAttribute
    {
        internal readonly object[] Range;

        public LabelCaseSolutionAttribute(object[] range)
        {
        }

        public LabelCaseSolutionAttribute(string name, object[] range) : base(name)
        {
            if (range == null || range.Length == 0) throw new ArgumentNullException(nameof(range));
            Range = range;
        }
    }
}
