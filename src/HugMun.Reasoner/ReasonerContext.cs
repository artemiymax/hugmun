using System;
using System.Collections.Generic;
using HugMun.Core;

namespace HugMun.Reasoning
{
    public class ReasonerContext
    {
        public ICaseFrame Cases { get; }

        public ICase TargetCase { get; }

        public CaseSchema Schema => Cases.Schema;

        public IEnumerable<CaseRetrievalResult> RetrievedCases { get; set; }

        public ReasonerContext(ICaseFrame cases, ICase targetCase)
        {
            Cases = cases ?? throw new ArgumentNullException(nameof(cases));
            TargetCase = targetCase ?? throw new ArgumentNullException(nameof(targetCase)); ;
        }
    }
}
