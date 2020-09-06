using System;
using HugMun.Core;

namespace HugMun.Data
{
    public class Case<TCase> : ICase where TCase : class
    {
        private bool disposed;

        private readonly CaseDefinition definition;
        private readonly Delegate idGetter;
        private readonly Delegate solutionGetter;
        private readonly Delegate[] getters;

        public TCase Data;

        public CaseSchema Schema { get; }

        public Case(TCase caseData, CaseSchema schema = null)
        {
            Data = caseData ?? throw new ArgumentNullException(nameof(caseData));
            definition = new CaseDefinitionBuilder(typeof(TCase), schema).Build();
            Schema = schema ?? definition.GetCaseSchema();
            idGetter = DynamicCodeUtils.GenerateGetter<TCase>(definition.IdAttribute);
            solutionGetter = DynamicCodeUtils.GenerateGetter<TCase>(definition.SolutionAttribute);
            getters = GenerateGetters();
        }

        internal Case(Delegate idGetter, Delegate solutionGetter, Delegate[] getters, CaseSchema schema)
        {
            Schema = schema;
            this.idGetter = idGetter;
            this.solutionGetter = solutionGetter;
            this.getters = GenerateGetters(getters);
        }

        public AttributeGetter<T> GetAttributeGetter<T>(AttributeSchema attribute)
        {
            var getter = getters[attribute.Index];
            if (!(getter is AttributeGetter<T> getterDelegate))
                throw new InvalidOperationException("Encountered incorrect getter type.");

            return getterDelegate;
        }

        public string GetId()
        {
            string result = default;
            if (!(idGetter is Getter<TCase, string> idGetterDelegate))
                throw new InvalidOperationException("Encountered incorrect id getter type.");

            idGetterDelegate(Data, ref result);
            return result;
        }

        public TSolution GetSolution<TSolution>()
        {
            TSolution result = default;
            if (!(solutionGetter is Getter<TCase, TSolution> solutionGetterDelegate))
                throw new InvalidOperationException("Encountered incorrect solution getter type.");

            solutionGetterDelegate(Data, ref result);
            return result;
        }

        private Delegate[] GenerateGetters(Delegate[] dynamicGetters = null)
        {
            Func<Delegate, Delegate> factory = GetterFactory<int>;

            var result = new Delegate[Schema.Count];
            for (var i = 0; i < Schema.Count; i++)
            {
                var getter = dynamicGetters != null ? dynamicGetters[i] : DynamicCodeUtils.GenerateGetter<TCase>(definition[i]);
                result[i] = DynamicCodeUtils.DelegateInvoke(factory, Schema[i].Type, getter, this);
            }

            return result;

            Delegate GetterFactory<T>(Delegate getter)
            {
                if (!(getter is Getter<TCase, T> getterDelegate))
                    throw new InvalidOperationException("Encountered incorrect getter type.");

                return (AttributeGetter<T>)((ref T value) => getterDelegate(Data, ref value));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            { }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
