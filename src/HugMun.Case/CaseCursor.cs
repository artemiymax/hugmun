namespace HugMun
{
    public abstract class CaseCursor : ICase
    {
        public abstract int Index { get; }

        public abstract CaseSchema Schema { get; }

        public abstract string GetId();

        public abstract AttributeGetter<T> GetAttributeGetter<T>(AttributeSchema attribute);

        public abstract TSolution GetSolution<TSolution>();

        public abstract bool MoveNext();

        public abstract void Dispose();

        protected virtual void Dispose(bool disposing)
        { }
    }
}
