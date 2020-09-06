using System;
using HugMun.Core;

namespace HugMun.Preprocessing
{
    internal class AttributeTransformingCursor : CaseCursor
    {
        private bool disposed;

        private readonly CaseCursor cursor;
        private readonly Delegate[] getters;
        private readonly AttributeBinding binding;

        public override int Index => cursor.Index;

        public override CaseSchema Schema => cursor.Schema;

        public AttributeTransformingCursor(CaseCursor originalCursor, AttributeTransformer transformer)
        {
            if (transformer == null) throw new ArgumentNullException(nameof(transformer));

            cursor = originalCursor ?? throw new ArgumentNullException(nameof(originalCursor));
            getters = transformer.GetTransformingGetters(originalCursor) ?? throw new ArgumentNullException(nameof(getters));
            binding = transformer.AttributeBinding ?? throw new ArgumentNullException(nameof(binding));
        }

        public override bool MoveNext() => cursor.MoveNext();

        public override string GetId() => cursor.GetId();

        public override AttributeGetter<T> GetAttributeGetter<T>(AttributeSchema attribute)
        {
            if (!binding.TryGetBinding(attribute.Index, out var index))
            {
                return cursor.GetAttributeGetter<T>(attribute);
            }

            var getter = getters[index];
            if (!(getter is AttributeGetter<T> getterDelegate))
                throw new InvalidOperationException("Encountered incorrect getter type.");

            return getterDelegate;
        }

        public override TSolution GetSolution<TSolution>() => cursor.GetSolution<TSolution>();
        public override void Dispose()
        {
            Dispose(true);
        }

        protected sealed override void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                cursor.Dispose();
            }

            disposed = true;
            base.Dispose(disposing);
        }
    }
}
