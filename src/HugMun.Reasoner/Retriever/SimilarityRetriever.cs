using System;
using System.Collections.Generic;
using System.Linq;
using HugMun.Core;
using HugMun.Preprocessing;

namespace HugMun.Reasoning
{
    public class SimilarityRetriever : IRetriever
    {
        private readonly ISimilarityMeasure measure;
        private readonly int threshold;

        public SimilarityRetriever(ISimilarityMeasure measure, int threshold)
        {
            if (threshold < 1)
                throw new ArgumentException("Case threshold should be greater than 0.", nameof(threshold));

            this.threshold = threshold;
            this.measure = measure ?? throw new ArgumentNullException(nameof(measure));
        }

        public IEnumerable<CaseRetrievalResult> Retrieve(ReasonerContext context)
        {
            var similaritySet = new SortedSet<CaseRetrievalResult>(new CaseRetrievalComparer());
            using (var cursor = GetTargetFrame(context).GetCaseCursor())
            {
                while (cursor.MoveNext())
                {
                    var similarity = measure.GetSimilarity(cursor, context.TargetCase, context);
                    var result = new CaseRetrievalResult(cursor.GetId(), similarity);
                    if (similaritySet.Count == threshold)
                    {
                        if (result.Measure >= similaritySet.Max.Measure) continue;
                        similaritySet.Remove(similaritySet.Max);
                        similaritySet.Add(result);
                    }
                    else
                    {
                        similaritySet.Add(result);
                    }
                }
            }

            return similaritySet;
        }

        private static ICaseFrame GetTargetFrame(ReasonerContext context)
        {
            if (context.RetrievedCases == null || !context.RetrievedCases.Any()) return context.Cases;

            var retrievedIds = context.RetrievedCases.Select(c => c.CaseId).ToArray();
            return context.Cases.FilterCases(retrievedIds);
        }
    }
}
