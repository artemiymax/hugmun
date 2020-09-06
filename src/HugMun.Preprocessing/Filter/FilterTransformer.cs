using System;
using HugMun.Core;

namespace HugMun.Preprocessing
{
    public sealed class FilterTransformer : ICaseTransformer
    {
        private readonly Func<ICase, bool> filter;

        public FilterTransformer(Func<ICase, bool> predicate)
        {
            filter = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        public void Prepare(ICaseFrame cases)
        {

        }

        public ICaseFrame Transform(ICaseFrame cases)
        {
            return new FilteringCaseFrame(cases, this);
        }

        internal Func<ICase, bool> GetFilter() => filter;
    }
}
