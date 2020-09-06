using System.Collections.Generic;
using System.Linq;
using HugMun.Reasoning.Lsh;

namespace HugMun.Reasoning
{
    public class LshRetriever : IRetriever
    {
        private readonly int size;
        private readonly int depth;

        private LshSpace lshSpace;

        public LshRetriever(int size, int depth = 1)
        {
            this.size = size;
            this.depth = depth;
        }

        public IEnumerable<CaseRetrievalResult> Retrieve(ReasonerContext context)
        {
            if (lshSpace == null) CreateLshSpace(context);
            return lshSpace
                .Get(context.TargetCase)
                .Select(id => new CaseRetrievalResult(id, 1));
        }

        private void CreateLshSpace(ReasonerContext context)
        {
            lshSpace = new LshSpace(size, context.Schema.Count, depth);
            using (var cursor = context.Cases.GetCaseCursor())
            {
                while (cursor.MoveNext())
                {
                    lshSpace.Add(cursor);
                }
            }
        }
    }
}
