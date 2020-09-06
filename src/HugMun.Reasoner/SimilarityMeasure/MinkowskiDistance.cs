using System;
using System.Linq;
using HugMun.Core;

namespace HugMun.Reasoning
{
    public class MinkowskiDistance : ISimilarityMeasure
    {
        private readonly double order;
        private readonly string[] targetAttributes;

        public MinkowskiDistance(double order, params string[] targetAttributes)
        {
            if (order < 1)
                throw new ArgumentException("Minkowski distance order must be a non-negative integer greater than 0.", nameof(order));

            this.order = order;
            this.targetAttributes = targetAttributes;
        }

        public double GetSimilarity(ICase caseA, ICase caseB, ReasonerContext context)
        {
            if (caseA == null) throw new ArgumentNullException(nameof(caseA));
            if (caseB == null) throw new ArgumentNullException(nameof(caseB));

            double result = 0;
            foreach (var attribute in caseA.Schema)
            {
                if (targetAttributes.Length > 0 && !targetAttributes.Contains(attribute.Name)) continue;
                result += Math.Pow(Math.Abs(caseA.GetAttribute<double>(attribute) - caseB.GetAttribute<double>(attribute)), order);
            }

            return Math.Pow(result, 1 / order);
        }
    }
}
