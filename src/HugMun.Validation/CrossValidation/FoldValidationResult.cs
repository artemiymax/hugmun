using System;
using System.Collections.Generic;

namespace HugMun.Validation
{
    public sealed class FoldValidationResult<TSolution>
    {
        public int FoldIndex { get; }

        public double ValidationTime { get; internal set; }

        public List<TSolution> GroundTruth { get; internal set; }

        public List<TSolution> Predicted { get; internal set; }

        internal FoldValidationResult(int foldIndex, int foldSize)
        {
            if (foldIndex < 0) throw new ArgumentOutOfRangeException(nameof(foldIndex));
            if (foldSize < 1) throw new ArgumentOutOfRangeException(nameof(foldSize), "Fold size should be greater than 0.");

            FoldIndex = foldIndex;
            GroundTruth = new List<TSolution>(foldSize);
            Predicted = new List<TSolution>(foldSize);
        }

        internal void AddPrediction(TSolution truth, TSolution prediction)
        {
            GroundTruth.Add(truth);
            Predicted.Add(prediction);
        }
    }
}
