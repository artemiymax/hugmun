using System;
using System.Collections.Generic;
using HugMun.Core;

namespace HugMun.Data
{
    public static class CollectionProvider
    {
        public static ICaseFrame Load<TCase>(IList<TCase> cases, CaseSchema schema = null) where TCase : class
        {
            if (cases == null) throw new ArgumentNullException(nameof(cases));
            return new EnumerableCaseFrame<TCase>(cases, schema);
        }
    }

    internal sealed class EnumerableCaseFrame<TCase> : TypedCaseFrame<TCase> where TCase : class
    {
        internal readonly IList<TCase> Data;

        public override int CaseCount => Data.Count;

        public EnumerableCaseFrame(IList<TCase> cases, CaseSchema schema) : base(schema)
        {
            Data = cases ?? throw new ArgumentNullException(nameof(cases));
        }

        public override CaseCursor GetCaseCursor() => new DelegatingCursor<TCase>(new EnumerableCursor<TCase>(this));
    }

    internal sealed class DelegatingCursor<TCase> : CaseCursor where TCase : class
    {
        private readonly TypedCursor<TCase> innerCursor;

        public override int Index => innerCursor.Index;

        public override CaseSchema Schema => innerCursor.Schema;

        public DelegatingCursor(TypedCursor<TCase> cursor)
        {
            innerCursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
        }

        public override bool MoveNext() => innerCursor.MoveNext();

        public override string GetId() => innerCursor.GetId();

        public override AttributeGetter<T> GetAttributeGetter<T>(AttributeSchema attribute) => innerCursor.GetAttributeGetter<T>(attribute);

        public override TSolution GetSolution<TSolution>() => innerCursor.GetSolution<TSolution>();

        public override void Dispose()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                innerCursor.Dispose();
        }
    }

    internal sealed class EnumerableCursor<TCase> : TypedCursor<TCase> where TCase : class
    {
        private readonly IEnumerator<TCase> enumerator;

        public EnumerableCursor(EnumerableCaseFrame<TCase> frame) : base(frame)
        {
            enumerator = frame.Data.GetEnumerator();
        }

        protected override bool MoveNextOuter()
        {
            var result = enumerator.MoveNext();
            Data = result ? enumerator.Current : null;
            return result;
        }
    }
}
