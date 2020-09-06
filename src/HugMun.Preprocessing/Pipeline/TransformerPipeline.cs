using System;
using System.Collections.Generic;
using System.Linq;
using HugMun.Core;

namespace HugMun.Preprocessing
{
    public sealed class TransformerPipeline : ICaseTransformer
    {
        private readonly List<ICaseTransformer> transformers;

        public TransformerPipeline()
        {
            transformers = new List<ICaseTransformer>();
        }

        public TransformerPipeline(ICaseTransformer transformer) : this()
        {
            Add(transformer);
        }

        public TransformerPipeline Add(ICaseTransformer transformer)
        {
            if (transformer == null) throw new ArgumentNullException(nameof(transformer));
            transformers.Add(transformer);

            return this;
        }

        public void Prepare(ICaseFrame cases)
        {
            foreach (var transformer in transformers)
            {
                transformer.Prepare(cases);
            }
        }

        public ICaseFrame Transform(ICaseFrame cases)
        {
            return transformers.Aggregate(cases, (current, transformer) => transformer.Transform(current));
        }
    }
}
