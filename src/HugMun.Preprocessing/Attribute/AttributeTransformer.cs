using System;
using HugMun.Core;

namespace HugMun.Preprocessing
{
    public abstract class AttributeTransformer : ICaseTransformer
    {
        internal abstract AttributeBinding AttributeBinding { get;  set; }

        internal abstract Delegate[] GetTransformingGetters(ICase targetCase);

        public abstract void Prepare(ICaseFrame cases);

        public abstract ICaseFrame Transform(ICaseFrame cases);
    }
}
