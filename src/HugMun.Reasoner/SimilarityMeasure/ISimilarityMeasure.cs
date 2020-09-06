using HugMun.Core;

namespace HugMun.Reasoning
{
    public interface ISimilarityMeasure
    {
        double GetSimilarity(ICase caseA, ICase caseB, ReasonerContext context);
    }
}