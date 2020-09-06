using System;

namespace HugMun.Core
{
    public readonly struct AttributeSchema
    {
        public string Name { get; }

        public int Index { get; }

        public Type Type { get; }

        internal AttributeSchema(string name, int index, Type type)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

            Name = name;
            Index = index;
            Type = type;
        }
    }
}
