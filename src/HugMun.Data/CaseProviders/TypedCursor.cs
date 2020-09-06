namespace HugMun.Data
{
    internal abstract class TypedCursor<TCase> : Case<TCase> where TCase : class
    {
        private bool disposed;

        internal int Index;

        protected TypedCursor(TypedCaseFrame<TCase> frame) : base(frame.IdGetter, frame.SolutionGetter, frame.Getters, frame.Schema)
        {
            Index = -1;
        }

        protected abstract bool MoveNextOuter();

        protected override void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                Index = -1;
            }

            disposed = true;
            base.Dispose(disposing);
        }

        public bool MoveNext()
        {
            if (disposed) return false;

            if (MoveNextOuter())
            {
                Index++;
                return true;
            }

            Dispose();
            return false;
        }
    }
}
