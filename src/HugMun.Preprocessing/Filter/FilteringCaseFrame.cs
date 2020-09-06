using System;
using HugMun.Core;

namespace HugMun.Preprocessing
{
    internal class FilteringCaseFrame : TransformingCaseFrame
    {
        private readonly FilterTransformer transformer;

        public override CaseSchema Schema => OriginalFrame.Schema;

        public override int CaseCount => OriginalFrame.CaseCount;

        public FilteringCaseFrame(ICaseFrame frame, FilterTransformer transformer) : base(frame)
        {
            this.transformer = transformer ?? throw new ArgumentNullException(nameof(transformer));
        }

        public override CaseCursor GetTransformingCursor()
        {
            return new FilteringCursor(OriginalFrame.GetCaseCursor(), transformer);
        }
    }

    internal class FilteringCursor : CaseCursor
    {
        private bool disposed;

        private readonly CaseCursor cursor;
        private readonly Func<ICase, bool> filter;

        public override int Index => cursor.Index;

        public override CaseSchema Schema => cursor.Schema;

        public FilteringCursor(CaseCursor originalCursor, FilterTransformer transformer)
        {
            if (transformer == null) throw new ArgumentNullException(nameof(transformer));

            cursor = originalCursor ?? throw new ArgumentNullException(nameof(originalCursor));
            filter = transformer.GetFilter() ?? throw new ArgumentNullException(nameof(filter));
        }

        public override bool MoveNext()
        {
            var result = cursor.MoveNext();
            while (result && !filter(cursor))
            {
                result = cursor.MoveNext();
            }

            return result;
        }

        public override string GetId() => cursor.GetId();

        public override AttributeGetter<T> GetAttributeGetter<T>(AttributeSchema attribute) => cursor.GetAttributeGetter<T>(attribute);

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
