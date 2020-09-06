using System;

namespace HugMun.Core
{
    public static class CaseExtensions
    {
        public static T GetAttribute<T>(this ICase targetCase, string name)
        {
            if (targetCase == null) throw new ArgumentNullException(nameof(targetCase));
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            var attribute = targetCase.Schema.TryGetAttribute(name) ??
                throw new ArgumentException($"Attribute {name} is not found.");

            return GetAttribute<T>(targetCase, attribute);
        }

        public static T GetAttribute<T>(this ICase targetCase, AttributeSchema attribute)
        {
            if(targetCase == null) throw new ArgumentNullException(nameof(targetCase));
            if(attribute.Type != typeof(T)) throw new ArgumentOutOfRangeException($"Attribute {attribute.Name} type is {attribute.Type}.");

            T value = default;
            var getter = targetCase.GetAttributeGetter<T>(attribute);
            getter(ref value);
            return value;
        }

        public static void CheckCompatibility(this ICase targetCase, ICase compatibleCase)
        {
            if (compatibleCase == null) throw new ArgumentNullException(nameof(compatibleCase));
            if (targetCase == null) throw new ArgumentNullException(nameof(targetCase));

            foreach (var aAttribute in compatibleCase.Schema)
            {
                CheckCompatibility(targetCase, aAttribute);
            }
        }

        public static void CheckCompatibility(this ICase targetCase, AttributeSchema attribute)
        {
            var caseAttribute = targetCase.Schema.TryGetAttribute(attribute.Name) ??
                throw new ArgumentException($"Cases are incompatible, attribute {attribute.Name} is missing.");

            if (caseAttribute.Type != attribute.Type)
                throw new ArgumentException($"Cases are incompatible, expected {attribute.Type} type for attribute {attribute.Name}.");
        }
    }
}
