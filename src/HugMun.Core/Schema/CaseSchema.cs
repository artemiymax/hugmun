using System;
using System.Collections;
using System.Collections.Generic;

namespace HugMun.Core
{
    public sealed class CaseSchema : IReadOnlyList<AttributeSchema>
    {
        private readonly AttributeSchema[] attributes;
        private readonly Dictionary<string, int> nameIndexMap;

        internal CaseSchema(AttributeSchema idAttribute, AttributeSchema solutionAttribute, AttributeSchema[] attributes, Dictionary<string, int> nameIndexMap)
        {
            IdAttribute = idAttribute;
            SolutionAttribute = solutionAttribute;
            this.attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));
            this.nameIndexMap = nameIndexMap ?? throw new ArgumentNullException(nameof(nameIndexMap));
        }

        public AttributeSchema IdAttribute { get; }

        public AttributeSchema SolutionAttribute { get; }

        public AttributeSchema this[int index]
        {
            get
            {
                if (index < 0 || index >= attributes.Length) throw new ArgumentOutOfRangeException(nameof(index));
                return attributes[index];
            }
        }

        public AttributeSchema this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
                if (!nameIndexMap.TryGetValue(name, out var index)) throw new ArgumentOutOfRangeException(nameof(name));
                return attributes[index];
            }
        }

        public AttributeSchema? TryGetAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            return nameIndexMap.TryGetValue(name, out var index) ? attributes[index] : (AttributeSchema?)null;
        }

        public int Count => attributes.Length;

        public IEnumerator<AttributeSchema> GetEnumerator() => ((IEnumerable<AttributeSchema>)attributes).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
