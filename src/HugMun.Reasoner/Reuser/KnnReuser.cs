using System;
using System.Collections.Generic;
using System.Linq;
using HugMun.Preprocessing;

namespace HugMun.Reasoning
{
    public class KnnReuser : IReuser
    {
        private readonly int k;
        private readonly Func<double, double> kernel;

        public KnnReuser(int neighborCount, Func<double, double> kernel = null)
        {
            if (neighborCount < 1)
                throw new ArgumentException("Closest neighbors count should be greater than 0.", nameof(neighborCount));

            k = neighborCount;
            this.kernel = kernel;
        }

        public TSolution Reuse<TSolution>(ReasonerContext context)
        {
            var nearestNeighbors = context.RetrievedCases
                .Take(k)
                .ToDictionary(c => c.CaseId, c => c.Measure);

            if (!nearestNeighbors.Any()) return default;

            var groupFrequency = new Dictionary<TSolution, double>();
            var filteredFrame = context.Cases.FilterCases(nearestNeighbors.Keys.ToArray());
            using (var cursor = filteredFrame.GetCaseCursor())
            {
                while (cursor.MoveNext())
                {
                    var group = cursor.GetSolution<TSolution>();
                    var distance = nearestNeighbors[cursor.GetId()];
                    var result = kernel?.Invoke(distance) ?? 1;

                    groupFrequency[group] = groupFrequency.TryGetValue(group, out var frequency)
                        ? frequency + result
                        : result;
                }
            }

            return groupFrequency
                .OrderByDescending(f => f.Value)
                .First()
                .Key;
        }
    }
}
