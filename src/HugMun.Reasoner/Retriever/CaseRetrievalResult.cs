using System;
using System.Collections.Generic;

namespace HugMun.Reasoning
{
    public readonly struct CaseRetrievalResult
    {
        public double Measure { get; }

        public string CaseId { get; }

        public CaseRetrievalResult(string caseId, double measure)
        {
            if (string.IsNullOrEmpty(caseId)) throw new ArgumentNullException(nameof(caseId));

            CaseId = caseId;
            Measure = measure;
        }
    }

    internal sealed class CaseRetrievalComparer : IComparer<CaseRetrievalResult>
    {
        public int Compare(CaseRetrievalResult x, CaseRetrievalResult y)
        {
            return x.Measure.CompareTo(y.Measure);
        }
    }
}
