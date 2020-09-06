using System;
using System.Collections.Generic;
using System.Linq;
using HugMun.Core;

namespace HugMun.Reasoning
{
    public class CosineDistance : ISimilarityMeasure
    {
        public double GetSimilarity(ICase caseA, ICase caseB, ReasonerContext context)
        {
            if (caseA == null) throw new ArgumentNullException(nameof(caseA));
            if (caseB == null) throw new ArgumentNullException(nameof(caseB));

            var attributesA = caseA.Schema.Select(caseA.GetAttribute<double>).ToArray();
            var attributesB = caseA.Schema.Select(caseB.GetAttribute<double>).ToArray();

            return Dot(attributesA, attributesB) / (Math.Pow(Dot(attributesA, attributesA), 2) * Math.Pow(Dot(attributesB, attributesB), 2));
        }

        private static double Dot(IEnumerable<double> a, IEnumerable<double> b)
        {
            return a.Zip(b, (itemA, itemB) => itemA * itemB).Sum();
        }
    }
}
