using System;

namespace HugMun.Case.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CaseIdAttribute : CaseAttributeAttribute
    {
        public CaseIdAttribute()
        {
        }

        public CaseIdAttribute(string name) : base(name)
        {
        }
    }
}
