using System;

namespace HugMun.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CaseAttributeAttribute : Attribute
    {
        internal readonly string Name;

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
