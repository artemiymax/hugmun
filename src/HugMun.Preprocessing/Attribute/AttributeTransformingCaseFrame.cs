using System;
using HugMun.Core;

namespace HugMun.Preprocessing
{
    internal class AttributeTransformingCaseFrame : TransformingCaseFrame
    {
        private readonly AttributeTransformer transformer;

        public override CaseSchema Schema => OriginalFrame.Schema;

        public override int CaseCount => OriginalFrame.CaseCount;

        public AttributeTransformingCaseFrame(ICaseFrame frame, AttributeTransformer transformer) : base(frame)
        {
            this.transformer = transformer ?? throw new ArgumentNullException(nameof(transformer));
        }

        public override CaseCursor GetTransformingCursor()
        {
            return new AttributeTransformingCursor(OriginalFrame.GetCaseCursor(), transformer);
        }
    }
}
