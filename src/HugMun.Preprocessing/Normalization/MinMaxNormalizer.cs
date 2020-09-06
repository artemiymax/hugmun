using System;
using System.Linq;
using HugMun.Core;

namespace HugMun.Preprocessing
{
    public class MinMaxNormalizer : AttributeTransformer
    {
        private readonly double minBound;
        private readonly double maxBound;
        private readonly string[] attributeNames;

        private bool isPrepared;
        private double[] attributeMin;
        private double[] attributeMax;

        internal override AttributeBinding AttributeBinding { get; set; }

        public MinMaxNormalizer(params string[] attributes) : this(0, 1, attributes)
        { }

        public MinMaxNormalizer(double min, double max, params string[] attributes)
        {
            if (min >= max) throw new ArgumentException("Minimum range should be lower than maximum.");
            if (attributes != null && attributes.Any(string.IsNullOrEmpty))
                throw new ArgumentException("Attribute names should be a non-empty string.");

            minBound = min;
            maxBound = max;
            attributeNames = attributes ?? Array.Empty<string>();
        }

        public override void Prepare(ICaseFrame cases)
        {
            if (cases == null) throw new ArgumentNullException(nameof(cases));
            BindSelectedAttributes(cases.Schema);

            using (var cursor = cases.GetCaseCursor())
            {
                while (cursor.MoveNext())
                {
                    foreach (var binding in AttributeBinding)
                    {
                        var getter = cursor.GetAttributeGetter<double>(cases.Schema[binding.Attribute]);
                        UpdateMinMax(binding.Binding, getter);
                    }
                }
            }

            isPrepared = true;
        }

        public override ICaseFrame Transform(ICaseFrame cases)
        {
            if (!isPrepared) throw new InvalidOperationException("Normalizer should be prepared before the transform.");
            if (cases == null) throw new ArgumentNullException(nameof(cases));

            return new AttributeTransformingCaseFrame(cases, this);
        }

        internal override Delegate[] GetTransformingGetters(ICase targetCase)
        {
            if (targetCase == null) throw new ArgumentNullException(nameof(targetCase));

            var getters = new AttributeGetter<double>[AttributeBinding.Count];

            foreach (var binding in AttributeBinding)
            {
                var min = attributeMin[binding.Binding];
                var max = attributeMax[binding.Binding];
                var attributeGetter = targetCase.GetAttributeGetter<double>(targetCase.Schema[binding.Attribute]);
                getters[binding.Binding] = (ref double value) =>
                {
                    attributeGetter(ref value);
                    Normalize(ref value, min, max);
                };
            }

            return getters;
        }

        private void BindSelectedAttributes(CaseSchema schema)
        {
            AttributeBinding = new AttributeBinding(schema, attributeNames);
            attributeMin = new double[AttributeBinding.Count];
            attributeMax = new double[AttributeBinding.Count];

            foreach (var binding in AttributeBinding)
            {
                if (schema[binding.Attribute].Type != typeof(double))
                    throw new InvalidOperationException($"Only {typeof(double)} attributes are allowed for normalizer.");

                attributeMin[binding.Binding] = double.PositiveInfinity;
                attributeMax[binding.Binding] = double.NegativeInfinity;
            }
        }

        private void UpdateMinMax(int index, AttributeGetter<double> getter)
        {
            double value = default;
            getter(ref value);

            attributeMin[index] = value < attributeMin[index] ? value : attributeMin[index];
            attributeMax[index] = value > attributeMax[index] ? value : attributeMax[index];
        }

        private void Normalize(ref double value, double min, double max)
        {
            value = minBound + (value - min) * (maxBound - minBound) / (max - min);
        }
    }
}
