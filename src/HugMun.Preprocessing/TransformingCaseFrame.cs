using System;
using HugMun.Core;

namespace HugMun.Preprocessing
{
    internal abstract class TransformingCaseFrame : ICaseFrame
    {
        protected readonly ICaseFrame OriginalFrame;

        public abstract CaseSchema Schema { get; }

        public abstract int CaseCount { get; }

        protected TransformingCaseFrame(ICaseFrame frame)
        {
            OriginalFrame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        public CaseCursor GetCaseCursor() => GetTransformingCursor();

        public abstract CaseCursor GetTransformingCursor();
    }
}
