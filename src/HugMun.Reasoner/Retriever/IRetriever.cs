using System.Collections.Generic;

namespace HugMun.Reasoning
{
    public interface IRetriever
    {
        IEnumerable<CaseRetrievalResult> Retrieve(ReasonerContext context);
    }
}