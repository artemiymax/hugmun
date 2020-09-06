using System;
using System.Collections.Generic;
using HugMun.Core;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;

namespace HugMun.Reasoning
{
    public class MahalanobisDistance : ISimilarityMeasure
    {
        private DenseMatrix covarianceMatrix;

        public MahalanobisDistance()
        { }

        public MahalanobisDistance(ICaseFrame cases)
        {
            if (cases == null) throw new ArgumentNullException(nameof(cases));
            covarianceMatrix = GetCovarianceMatrix(cases);
        }

        public MahalanobisDistance(double[,] covarianceMatrix)
        {
            if (covarianceMatrix == null) throw new ArgumentNullException(nameof(covarianceMatrix));
            if (covarianceMatrix.GetLength(0) != covarianceMatrix.GetLength(1))
                throw new ArgumentException("Covariance matrix should be a square matrix.");

            this.covarianceMatrix = DenseMatrix.OfArray(covarianceMatrix);
        }

        public double GetSimilarity(ICase caseA, ICase caseB, ReasonerContext context)
        {
            if (covarianceMatrix == null) covarianceMatrix = GetCovarianceMatrix(context.Cases);
            if (caseA == null) throw new ArgumentNullException(nameof(caseA));
            if (caseB == null) throw new ArgumentNullException(nameof(caseB));

            var differenceVector = new DenseMatrix(1, caseA.Schema.Count);
            for (var i = 0; i < caseA.Schema.Count; i++)
            {
                differenceVector[0, i] = caseA.GetAttribute<double>(caseA.Schema[i]) - caseB.GetAttribute<double>(caseA.Schema[i]);
            }

            var distance = differenceVector * covarianceMatrix.Transpose() * differenceVector.Transpose();
            return Math.Sqrt(distance[0, 0]);
        }

        private static DenseMatrix GetCovarianceMatrix(ICaseFrame cases)
        {
            var data = new List<double[]>();
            var cursor = cases.GetCaseCursor();
            while (cursor.MoveNext())
            {
                var attributes = new double[cases.Schema.Count];
                for (var i = 0; i < cases.Schema.Count; i++)
                {
                    attributes[i] = cursor.GetAttribute<double>(cases.Schema[i]);
                }
                data.Add(attributes);
            }

            var dataMat = DenseMatrix.Build.DenseOfRowArrays(data);
            var covMat = new DenseMatrix(dataMat.ColumnCount, dataMat.ColumnCount);
            for (var i = 0; i < dataMat.ColumnCount; i++)
            {
                for (var j = 0; j < dataMat.ColumnCount; j++)
                {
                    covMat[i, j] = dataMat.Column(i).Covariance(dataMat.Column(j));
                }
            }

            return covMat;
        }
    }
}
