using System;
using HugMun.Case;

namespace HugMun.Similarity
{
    public class MinkowskiDistance : ISimilarityMeasure
    {
        private readonly double order;

        public MinkowskiDistance(double order)
        {
            if (order < 1)
                throw new ArgumentException("Minkowski distance order must be a non-negative integer greater than 0.", nameof(order));

            this.order = order;
        }

        public double GetSimilarity(ICase caseA, ICase caseB)
        {
            if (caseA == null) throw new ArgumentNullException(nameof(caseA));
            if (caseB == null) throw new ArgumentNullException(nameof(caseB));

            double result = 0;
            foreach (var attribute in caseA.Schema)
            {
                result += attribute.Type == typeof(double)
                    ? Math.Pow(Math.Abs(caseA.GetAttribute<double>(attribute) - caseB.GetAttribute<double>(attribute)), order)
                    : 0;
            }

            return Math.Pow(result, 1 / order);
        }
    }
}
