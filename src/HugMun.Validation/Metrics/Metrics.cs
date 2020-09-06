using System;
using System.Collections.Generic;
using System.Linq;

namespace HugMun.Validation
{
    public static class Metrics
    {
        public static double Accuracy<TSolution>(this CrossValidationResult<TSolution> result, IEqualityComparer<TSolution> comparer = null)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            var correct = 0;
            var overall = 0;
            var solutionComparer = comparer ?? EqualityComparer<TSolution>.Default;
            foreach (var fold in result.Folds)
            {
                overall += fold.GroundTruth.Count;
                correct += fold.GroundTruth.Where((t, i) => solutionComparer.Equals(t, fold.Predicted[i])).Count();
            }

            return overall != 0 ? (double)correct / overall : 0;
        }

        public static ConfusionMatrix<TSolution> ConfusionMatrix<TSolution>(this CrossValidationResult<TSolution> result, IComparer<TSolution> comparer = null)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            var solutionComparer = comparer ?? Comparer<TSolution>.Default;
            var matrix = new SortedDictionary<TSolution, SortedDictionary<TSolution, int>>(solutionComparer);

            foreach (var fold in result.Folds)
            {
                for (var i = 0; i < fold.GroundTruth.Count; i++)
                {
                    var truth = fold.GroundTruth[i];
                    var prediction = fold.Predicted[i];
                    if (!matrix.TryGetValue(truth, out var truthMap))
                    {
                        truthMap = new SortedDictionary<TSolution, int>(solutionComparer) {[truth] = 0};
                        matrix[truth] = truthMap;
                    }

                    truthMap[prediction] = !truthMap.TryGetValue(prediction, out var count) ? 1 : count + 1;

                    if (matrix.TryGetValue(prediction, out _)) continue;
                    matrix[prediction] = new SortedDictionary<TSolution, int>(solutionComparer) { [prediction] = 0 };
                }
            }

            return new ConfusionMatrix<TSolution>(matrix);
        }
    }
}
