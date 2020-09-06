using System;
using System.Collections.Generic;
using System.Linq;

namespace HugMun.Reasoning
{
    public class VotingRetriever : IRetriever
    {
        private readonly IEnumerable<IRetriever> retrievers;
        private readonly int threshold;

        public VotingRetriever(int threshold, params IRetriever[] retrievers)
        {
            if (threshold < 1)
                throw new ArgumentException("Case threshold should be greater than 0.", nameof(threshold));

            this.retrievers = retrievers;
            this.threshold = threshold;
        }

        public IEnumerable<CaseRetrievalResult> Retrieve(ReasonerContext context)
        {
            var cases = new Dictionary<string, (double count, double measure)>();
            foreach (var retriever in retrievers)
            {
                var retrievedCases = retriever.Retrieve(context).ToArray();
                for (var i = 0; i < retrievedCases.Length; i++)
                {
                    var retrievedCase = retrievedCases[i];
                    if (!cases.TryGetValue(retrievedCase.CaseId, out var targetCase))
                    {
                        cases[retrievedCase.CaseId] = (i, retrievedCase.Measure);
                    }
                    else
                    {
                        cases[retrievedCase.CaseId] = (targetCase.count + i, targetCase.measure + retrievedCase.Measure);
                    }
                }
            }

            return cases.Select(c => new CaseRetrievalResult(c.Key, c.Value.count))
                .OrderBy(c => c.Measure)
                .Take(threshold);
        }
    }
}
