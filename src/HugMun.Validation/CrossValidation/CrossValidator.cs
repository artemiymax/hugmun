using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HugMun.Core;
using HugMun.Preprocessing;
using HugMun.Reasoning;

namespace HugMun.Validation
{
    public static class CrossValidator
    {
        public static string[][] CreateFolds(ICaseFrame cases, int foldCount, bool shuffle)
        {
            if (cases == null)
                throw new ArgumentNullException(nameof(cases));
            if (foldCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(foldCount), "Fold count should be greater than 0.");

            var ids = GetIds(cases, shuffle).ToArray();
            var foldSize = Math.DivRem(ids.Length, foldCount, out var upperFoldCount);
            var folds = new string[foldCount][];

            var idIndex = 0;
            for (var i = 0; i < foldCount; i++)
            {
                folds[i] = i < upperFoldCount ? new string[foldSize + 1] : new string[foldSize];
                for (var j = 0; j < folds[i].Length; j++)
                {
                    folds[i][j] = ids[idIndex];
                    idIndex += 1;
                }
            }

            return folds;
        }

        public static CrossValidationResult<TSolution> Validate<TSolution>(string[][] folds, ICaseFrame cases, Reasoner reasoner, ICaseTransformer pipeline = null)
        {
            if (folds == null) throw new ArgumentNullException(nameof(folds));
            if (cases == null) throw new ArgumentNullException(nameof(cases));
            if (reasoner == null) throw new ArgumentNullException(nameof(reasoner));

            var results = new FoldValidationResult<TSolution>[folds.Length];
            for (var i = 0; i < folds.Length; i++)
            {
                results[i] = ProcessFold<TSolution>(folds, i, cases, pipeline, reasoner);
            }

            return new CrossValidationResult<TSolution>(results);
        }

        public static CrossValidationResult<TSolution> ValidateInParallel<TSolution>(string[][] folds, ICaseFrame cases, Func<Reasoner> reasonerBuilder, Func<ICaseTransformer> pipelineBuilder = null, ParallelOptions options = null)
        {
            if (folds == null) throw new ArgumentNullException(nameof(folds));
            if (cases == null) throw new ArgumentNullException(nameof(cases));
            if (reasonerBuilder == null) throw new ArgumentNullException(nameof(reasonerBuilder));

            var results = new FoldValidationResult<TSolution>[folds.Length];
            Parallel.For(0, folds.Length, options ?? new ParallelOptions(), i =>
            {
                results[i] = ProcessFold<TSolution>(folds, i, cases, pipelineBuilder?.Invoke(), reasonerBuilder());
            });

            return new CrossValidationResult<TSolution>(results);
        }

        private static FoldValidationResult<TSolution> ProcessFold<TSolution>(string[][] folds, int testFoldIndex, ICaseFrame cases, ICaseTransformer pipeline, Reasoner reasoner)
        {
            var watch = Stopwatch.StartNew();
            var result = new FoldValidationResult<TSolution>(testFoldIndex, folds[testFoldIndex].Length);

            var testFold = cases.FilterCases(folds[testFoldIndex]);
            var trainFold = cases.FilterCases(folds.Where((f, i) => i != testFoldIndex).SelectMany(f => f));

            pipeline.Prepare(trainFold);
            testFold = pipeline.Transform(testFold);
            trainFold = pipeline.Transform(trainFold);

            reasoner.SetData(trainFold);

            using (var cursor = testFold.GetCaseCursor())
            {
                while (cursor.MoveNext())
                {
                    var trueSolution = cursor.GetSolution<TSolution>();
                    var predictedSolution = reasoner.GetSolution<TSolution>(cursor);
                    result.AddPrediction(trueSolution, predictedSolution);
                }
            }
            watch.Stop();
            result.ValidationTime = watch.ElapsedMilliseconds;

            return result;
        }

        private static IEnumerable<string> GetIds(ICaseFrame cases, bool shuffle)
        {
            var caseIds = new List<string>();
            using (var cursor = cases.GetCaseCursor())
            {
                while (cursor.MoveNext())
                {
                    caseIds.Add(cursor.GetId());
                }
            }

            return shuffle
                ? caseIds.OrderBy(a => Guid.NewGuid())
                : (IEnumerable<string>)caseIds;
        }
    }
}
