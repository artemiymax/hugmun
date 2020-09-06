using System;
using System.Collections;
using System.Collections.Generic;

namespace HugMun.Core
{
    public class CaseDefinition : IReadOnlyList<AttributeDefinition>
    {
        private readonly AttributeDefinition[] attributes;
        private readonly Dictionary<string, int> nameIndexMap;

        internal CaseDefinition(AttributeDefinition idAttribute, AttributeDefinition solutionAttribute, AttributeDefinition[] attributes, Dictionary<string, int> nameIndexMap)
        {
            IdAttribute = idAttribute ?? throw new ArgumentNullException(nameof(idAttribute));
            SolutionAttribute = solutionAttribute ?? throw new ArgumentNullException(nameof(solutionAttribute));
            this.attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));
            this.nameIndexMap = nameIndexMap ?? throw new ArgumentNullException(nameof(nameIndexMap));
        }

        public AttributeDefinition IdAttribute { get; }

        public AttributeDefinition SolutionAttribute { get; }

        public AttributeDefinition this[int index]
        {
            get
            {
                if (index < 0 || index >= attributes.Length) throw new ArgumentOutOfRangeException(nameof(index));
                return attributes[index];
            }
        }

        public AttributeDefinition this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
                if (!nameIndexMap.TryGetValue(name, out var index)) throw new ArgumentOutOfRangeException(nameof(name));
                return attributes[index];
            }
        }

        public int Count => attributes.Length;

        public IEnumerator<AttributeDefinition> GetEnumerator() => ((IEnumerable<AttributeDefinition>)attributes).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
