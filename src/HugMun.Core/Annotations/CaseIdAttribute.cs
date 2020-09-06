using System;

namespace HugMun.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CaseIdAttribute : Attribute
    {
        internal readonly string Name;

        public CaseIdAttribute()
        {
        }

        public CaseIdAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            Name = name;
        }
    }
}
