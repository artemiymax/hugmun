using System;
using System.Collections.Generic;
using HugMun.Core;

namespace HugMun.Preprocessing
{
    public static class TransformExtensions
    {
        public static ICaseFrame FilterCases(this ICaseFrame caseFrame, IEnumerable<string> caseIds)
        {
            if (caseFrame == null) throw new ArgumentNullException(nameof(caseFrame));
            if (caseIds == null) throw new ArgumentNullException(nameof(caseIds));

            var ids = new HashSet<string>(caseIds);

            var filterTransformer = new FilterTransformer(c => ids.Contains(c.GetId()));
            filterTransformer.Prepare(caseFrame);

            return filterTransformer.Transform(caseFrame);
        }
    }
}
