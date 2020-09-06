using System;
using System.Collections.Generic;
using System.Linq;
using HugMun.Core;

namespace HugMun.Reasoning.Lsh
{
    public sealed class LshSpace
    {
        private readonly LshTable[] tables;

        public LshSpace(int size, int dimensions, int depth = 1)
        {
            if (size <= 0) throw new ArgumentException("LSH table size should be larger than 0.");
            if (dimensions <= 0) throw new ArgumentException("LSH table dimensions should be larger than 0.");
            if (depth <= 0) throw new ArgumentException("LSH space depth should be larger than 0.");

            tables = new LshTable[depth];
            for (var i = 0; i < depth; i++)
            {
                tables[i] = new LshTable(size, dimensions);
            }
        }

        public void Add(ICase value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            foreach (var table in tables)
            {
                table.Add(value);
            }
        }

        public IEnumerable<string> Get(ICase value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var result = new List<string>();
            foreach (var table in tables)
            {
                result.AddRange(table.Get(value));
            }

            return result.Distinct();
        }
    }
}
