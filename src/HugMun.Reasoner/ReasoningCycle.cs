using System;
using System.Collections.Generic;

namespace HugMun.Reasoning
{
    public sealed class ReasoningCycle
    {
        internal ICollection<IRetriever> Retrievers;
        internal IReuser Reuser;

        public ReasoningCycle()
        {
            Retrievers = new List<IRetriever>();
        }

        public ReasoningCycle(ICollection<IRetriever> caseRetrievers, IReuser caseReuser)
        {
            Retrievers = caseRetrievers ?? new List<IRetriever>();
            Reuser = caseReuser;
        }

        public ReasoningCycle AddRetriever(IRetriever caseRetriever)
        {
            if (caseRetriever == null) throw new ArgumentNullException(nameof(caseRetriever));
            Retrievers.Add(caseRetriever);
            return this;
        }

        public ReasoningCycle SetReuser(IReuser caseReuser)
        {
            Reuser = caseReuser ?? throw new ArgumentNullException(nameof(caseReuser));
            return this;
        }
    }
}
