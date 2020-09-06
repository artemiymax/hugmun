using System;
using HugMun.Core;

namespace HugMun.Reasoning
{
    public class Reasoner
    {
        private readonly ReasoningCycle cycle;
        private ICaseFrame data;

        public Reasoner(ReasoningCycle reasoningCycle)
        {
            cycle = reasoningCycle ?? throw new ArgumentNullException(nameof(reasoningCycle));
        }

        public void SetData(ICaseFrame cases)
        {
            data = cases ?? throw new ArgumentNullException(nameof(cases));
        }

        public TSolution GetSolution<TSolution>(ICase targetCase)
        {
            if (data == null) throw new InvalidOperationException("Case data for the reasoner is not set.");
            var context = new ReasonerContext(data, targetCase);

            foreach (var retriever in cycle.Retrievers)
            {
                var retrievedCases = retriever.Retrieve(context);
                context.RetrievedCases = retrievedCases;
            }

            var result = cycle.Reuser != null ? cycle.Reuser.Reuse<TSolution>(context) : default;
            return result;
        }
    }
}
