using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HugMun.Core
{
    public sealed class CaseDefinitionBuilder
    {
        private readonly Type targetType;
        private readonly CaseSchema targetSchema;

        public CaseDefinitionBuilder(Type type, CaseSchema schema) : this(type)
        {
            targetSchema = schema;
        }

        public CaseDefinitionBuilder(Type type)
        {
            targetType = type ?? throw new ArgumentNullException(nameof(type));
        }

        public CaseDefinition Build()
        {
            var properties = targetType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .ToArray();

            AttributeDefinition idDefinition = null;
            AttributeDefinition solutionDefinition = null;
            var attributeDefinitions = new List<AttributeDefinition>();
            var nameIndexMap = new Dictionary<string, int>();

            if (targetSchema != null)
            {
                idDefinition = GetSchemaDefinition(targetSchema.IdAttribute, properties);
                solutionDefinition = GetSchemaDefinition(targetSchema.SolutionAttribute, properties);
                foreach (var attribute in targetSchema)
                {
                    var definition = GetSchemaDefinition(attribute, properties);
                    attributeDefinitions.Add(definition);
                    UpdateIndexMap(nameIndexMap, definition, attribute.Index);
                }
            }
            else
            {
                foreach (var property in properties)
                {
                    var idAttribute = property.GetCustomAttribute<CaseIdAttribute>();
                    var attributeAttribute = property.GetCustomAttribute<CaseAttributeAttribute>();
                    var solutionAttribute = property.GetCustomAttribute<CaseSolutionAttribute>();

                    if (idAttribute != null)
                    {
                        if(idDefinition != null)
                            throw new ArgumentOutOfRangeException(nameof(targetType), "Only a single case ID attribute can be specified.");
                        if (attributeAttribute != null || solutionAttribute != null)
                            throw new ArgumentOutOfRangeException(nameof(targetType), "Only ID, case attribute or solution attribute can be specified for a member.");

                        idDefinition = new AttributeDefinition(idAttribute.Name ?? property.Name, property.PropertyType, property);
                    }
                    if (attributeAttribute != null)
                    {
                        if (solutionAttribute != null)
                            throw new ArgumentOutOfRangeException(nameof(targetType), "Only ID, attribute or solution attribute can be specified for a member.");

                        var definition = new AttributeDefinition(attributeAttribute.Name ?? property.Name, property.PropertyType, property);
                        attributeDefinitions.Add(definition);
                        UpdateIndexMap(nameIndexMap, definition, attributeDefinitions.Count - 1);
                    }
                    if (solutionAttribute != null)
                    {
                        if (solutionDefinition != null)
                            throw new ArgumentOutOfRangeException(nameof(targetType), "Only a single case solution attribute can be specified.");

                        solutionDefinition = new AttributeDefinition(solutionAttribute.Name ?? property.Name, property.PropertyType, property);
                    }
                }
            }

            return new CaseDefinition(idDefinition, solutionDefinition, attributeDefinitions.ToArray(), nameIndexMap);
        }

        private AttributeDefinition GetSchemaDefinition(AttributeSchema schema, IEnumerable<PropertyInfo> properties)
        {
            var schemaProperty = properties.FirstOrDefault(p => p.Name == schema.Name);

            if (schemaProperty == null)
                throw new InvalidOperationException($"Type {targetType} should contain public member {schema.Name}");
            if (schemaProperty.PropertyType != schema.Type)
                throw new ArgumentException($"Property type {schemaProperty.PropertyType} differs from schema type {schema.Type}");

            return new AttributeDefinition(schemaProperty.Name, schemaProperty.PropertyType, schemaProperty);
        }

        private static void UpdateIndexMap(Dictionary<string, int> map, AttributeDefinition definition, int index)
        {
            if (map.ContainsKey(definition.Name))
                throw new ArgumentException($"Attribute definition {definition.Name} is already defined");

            map.Add(definition.Name, index);
        }
    }
}
