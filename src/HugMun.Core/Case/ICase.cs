using System;

namespace HugMun.Core
{
    public delegate void AttributeGetter<T>(ref T attribute);

    public interface ICase : IDisposable
    {
        CaseSchema Schema { get; }

        string GetId();

        AttributeGetter<T> GetAttributeGetter<T>(AttributeSchema attribute);

        TSolution GetSolution<TSolution>();
    }
}
