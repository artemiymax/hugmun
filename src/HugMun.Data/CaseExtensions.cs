using System;
using System.Collections.Generic;
using HugMun.Core;

namespace HugMun.Data
{
    public static class CaseExtensions
    {
        public static IEnumerable<TCase> ToEnumerable<TCase>(this ICaseFrame caseFrame) where TCase : class, new()
        {
            if(caseFrame == null) throw new ArgumentNullException();

            using (var cursor = new WriteCursor<TCase>(caseFrame))
            {
                while (cursor.MoveNext())
                {
                    var targetCase = new TCase();
                    cursor.CreateCase(targetCase);

                    yield return targetCase;
                }
            }
        }
    }

    internal class WriteCursor<TCase> : CaseCursor where TCase : class
    {
        private bool disposed;

        private readonly CaseCursor cursor;
        private readonly CaseDefinition definition;
        private readonly Action<TCase> idSetter;
        private readonly Action<TCase> solutionSetter;
        private readonly Action<TCase>[] setters;

        public override int Index => cursor.Index;

        public override CaseSchema Schema => cursor.Schema;

        public WriteCursor(ICaseFrame caseFrame)
        {
            if(caseFrame == null) throw new ArgumentNullException(nameof(caseFrame));

            cursor = caseFrame.GetCaseCursor();
            definition = new CaseDefinitionBuilder(typeof(TCase), cursor.Schema).Build();
            idSetter = GenerateIdSetter();
            solutionSetter = GenerateSolutionSetter();
            setters = GenerateSetters();
        }

        public override bool MoveNext() => cursor.MoveNext();

        public override string GetId() => cursor.GetId();

        public override AttributeGetter<T> GetAttributeGetter<T>(AttributeSchema attribute) => cursor.GetAttributeGetter<T>(attribute);

        public override TSolution GetSolution<TSolution>() => cursor.GetSolution<TSolution>();

        internal void CreateCase(TCase targetCase)
        {
            idSetter(targetCase);
            foreach (var setter in setters)
            {
                setter(targetCase);
            }
            solutionSetter(targetCase);
        }

        private Action<TCase>[] GenerateSetters()
        {
            var result = new Action<TCase>[Schema.Count];
            for (var i = 0; i < Schema.Count; i++)
            {
                result[i] = GenerateSetter(Schema[i], definition[i]);
            }

            return result;
        }

        private Action<TCase> GenerateSetter(AttributeSchema attributeSchema, AttributeDefinition attributeDefinition)
        {
            Func<Delegate, Action<TCase>> factory = SetterFactory<int>;

            var setter = DynamicCodeUtils.GenerateSetter<TCase>(attributeDefinition);
            return DynamicCodeUtils.DelegateInvoke(factory, attributeDefinition.Type, setter, this);

            Action<TCase> SetterFactory<T>(Delegate generatedSetter)
            {
                if (!(generatedSetter is Setter<TCase, T> setterDelegate))
                    throw new InvalidOperationException("Encountered incorrect setter type.");

                var getter = cursor.GetAttributeGetter<T>(attributeSchema);
                return target =>
                {
                    T value = default;
                    getter(ref value);
                    setterDelegate(target, value);
                };
            }
        }

        private Action<TCase> GenerateIdSetter()
        {
            var setter = DynamicCodeUtils.GenerateSetter<TCase>(definition.IdAttribute);
            if (!(setter is Setter<TCase, string> setterDelegate))
                throw new InvalidOperationException("Encountered incorrect ID setter type.");

            return target =>
            {
                var value = cursor.GetId();
                setterDelegate(target, value);
            };
        }

        private Action<TCase> GenerateSolutionSetter()
        {
            Func<Delegate, Action<TCase>> factory = SetterFactory<int>;
            var setter = DynamicCodeUtils.GenerateSetter<TCase>(definition.SolutionAttribute);
            return DynamicCodeUtils.DelegateInvoke(factory, definition.SolutionAttribute.Type, setter, this);

            Action<TCase> SetterFactory<T>(Delegate generatedSetter)
            {
                if (!(generatedSetter is Setter<TCase, T> setterDelegate))
                    throw new InvalidOperationException("Encountered incorrect solution setter type.");

                return target =>
                {
                    var value = cursor.GetSolution<T>();
                    setterDelegate(target, value);
                };
            }
        }

        public override void Dispose()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
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
