using System;
using System.Reflection;

namespace HugMun.Core
{
    public class AttributeDefinition
    {
        public string Name { get; }

        public MemberInfo MemberInfo { get; }

        public Type Type { get; }

        internal AttributeDefinition(string name, Type type, MemberInfo memberInfo)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Name = name;
            MemberInfo = memberInfo ?? throw new ArgumentNullException(nameof(memberInfo));
            Type = type;
        }
    }
}
