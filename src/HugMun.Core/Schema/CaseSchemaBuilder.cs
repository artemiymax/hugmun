using System;
using System.Collections.Generic;

namespace HugMun.Core
{
    public sealed class CaseSchemaBuilder
    {
        private readonly List<(string name, Type type)> attributes;
        private string idAttribute;
        private (string name, Type type) solutionAttribute;

        public CaseSchemaBuilder()
        {
            attributes = new List<(string, Type)>();
        }

        public CaseSchemaBuilder AddId(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            idAttribute = name;
            return this;
        }

        public CaseSchemaBuilder AddSolution(string name, Type type)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            solutionAttribute = (name, type);
            return this;
        }

        public CaseSchemaBuilder AddAttribute(string name, Type type)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            attributes.Add((name, type));
            return this;
        }

        public CaseSchema Build()
        {
            if (string.IsNullOrEmpty(idAttribute)) throw new InvalidOperationException("Id attribute is missing.");
            if (string.IsNullOrEmpty(solutionAttribute.name)) throw new InvalidOperationException("Solution attribute is missing.");

            var attributeSchemas = new AttributeSchema[attributes.Count];
            var nameIndexMap = new Dictionary<string, int>();

            for (var i = 0; i < attributes.Count; i++)
            {
                attributeSchemas[i] = new AttributeSchema(attributes[i].name, i, attributes[i].type);

                if (nameIndexMap.ContainsKey(attributes[i].name))
                    throw new ArgumentException($"Attribute schema {attributes[i].name} is already defined");

                nameIndexMap.Add(attributes[i].name, i);
            }
            return new CaseSchema
            (
                new AttributeSchema(idAttribute, 0, typeof(string)),
                new AttributeSchema(solutionAttribute.name, 0, solutionAttribute.type),
                attributeSchemas,
                nameIndexMap
            );
        }
    }
}
