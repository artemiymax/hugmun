using System;
using System.Collections;
using System.Collections.Generic;
using HugMun.Core;

namespace HugMun.Preprocessing
{
    internal class AttributeBinding : IReadOnlyList<BindingPair>
    {
        private readonly BindingPair[] bindingPairs;
        private readonly Dictionary<int, int> attributeBinding;

        public int Count => bindingPairs.Length;

        public BindingPair this[int index] => bindingPairs[index];

        public AttributeBinding(CaseSchema schema, string[] attributes = null)
        {
            if (schema == null) throw new ArgumentNullException(nameof(schema));
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));
            if (attributes.Length > schema.Count) throw new InvalidOperationException("Attribute count exceeds schema.");

            attributeBinding = new Dictionary<int, int>();
            if (attributes.Length == 0)
            {
                bindingPairs = new BindingPair[schema.Count];
                for (var i = 0; i < schema.Count; i++)
                {
                    bindingPairs[i] = new BindingPair(i, schema[i].Index);
                    attributeBinding[schema[i].Index] = i;
                }
            }
            else
            {
                bindingPairs = new BindingPair[attributes.Length];
                for (var i = 0; i < attributes.Length; i++)
                {
                    var attribute = schema.TryGetAttribute(attributes[i]);
                    if (attribute == null)
                        throw new InvalidOperationException($"Selected attribute {attributes[i]} is missing in schema.");

                    bindingPairs[i] = new BindingPair(i, attribute.Value.Index);
                    attributeBinding[attribute.Value.Index] = i;
                }
            }
        }

        public bool TryGetBinding(int attributeIndex, out int bindingIndex)
        {
            return attributeBinding.TryGetValue(attributeIndex, out bindingIndex);
        }

        public IEnumerator<BindingPair> GetEnumerator() => ((IEnumerable<BindingPair>)bindingPairs).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal class BindingPair
    {
        public int Binding { get; }

        public int Attribute { get; }

        public BindingPair(int bindingIndex, int attributeIndex)
        {
            Binding = bindingIndex;
            Attribute = attributeIndex;
        }
    }
}
