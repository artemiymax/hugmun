using System;

namespace HugMun.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CaseSolutionAttribute : Attribute
    {
        internal readonly string Name;

        public CaseSolutionAttribute()
        {
        }

        public CaseSolutionAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            Name = name;
        }
    }
}
