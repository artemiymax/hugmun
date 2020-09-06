using System;
using System.Collections.Generic;
using HugMun.Core;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;

namespace HugMun.Reasoning
{
    public sealed class LshTable
    {
        private readonly Dictionary<int, List<string>> table;
        private readonly Vector[] projectionVectors;

        public LshTable(int size, int dimensions)
        {
            if (size <= 0) throw new ArgumentException("LSH table size should be larger than 0.");
            if (dimensions <= 0) throw new ArgumentException("LSH table dimensions should be larger than 0.");

            table = new Dictionary<int, List<string>>();
            projectionVectors = new Vector[size];

            var normalDistrib = new Normal();
            for (var i = 0; i < size; i++)
            {
                var projectionSample = new double[dimensions];
                normalDistrib.Samples(projectionSample);
                projectionVectors[i] = new DenseVector(projectionSample);
            }
        }

        public void Add(ICase value)
        {
            var hash = GenerateHash(value);
            if (!table.TryGetValue(hash, out var values))
            {
                table[hash] = new List<string> { value.GetId() };
            }
            else
            {
                values.Add(value.GetId());
            }
        }

        public List<string> Get(ICase value)
        {
            var hash = GenerateHash(value);
            return !table.TryGetValue(hash, out var values)
                ? new List<string>()
                : values;
        }

        private int GenerateHash(ICase value)
        {
            var hash = 0;

            var caseVector = GetAttributesVector(value);
            for (var i = 0; i < projectionVectors.Length; i++)
            {
                var product = projectionVectors[i].DotProduct(caseVector);
                if (product > 0) hash += 1 << i;
            }

            return hash;
        }

        private static Vector GetAttributesVector(ICase targetCase)
        {
            var attributes = new double[targetCase.Schema.Count];
            for (var i = 0; i < targetCase.Schema.Count; i++)
            {
                attributes[i] = targetCase.GetAttribute<double>(targetCase.Schema[i]);
            }

            return new DenseVector(attributes);
        }
    }
}
