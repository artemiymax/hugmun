using System;

namespace HugMun.Case.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CaseAttributeAttribute : Attribute
    {
        public readonly string Name;

        public CaseAttributeAttribute()
        {
        }

        public CaseAttributeAttribute(string name)
        {
            if(string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            Name = name;
        }
    }
}
