
namespace HugMun.Similarity
{
    public interface ISimilarityMeasure
    {
        double GetSimilarity(ICase caseA, ICase caseB, ReasonerContext context);
    }
}