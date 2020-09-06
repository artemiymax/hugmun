using System;
using System.Linq;

namespace HugMun.Validation
{
    public sealed class CrossValidationResult<TSolution>
    {
        public FoldValidationResult<TSolution>[] Folds { get; }

        public double ValidationTime => Folds.Average(f => f.ValidationTime);

        internal CrossValidationResult(FoldValidationResult<TSolution>[] foldResults)
        {
            Folds = foldResults ?? throw new ArgumentNullException(nameof(foldResults));
        }
    }
}
