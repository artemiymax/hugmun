using System;
using HugMun.Core;

namespace HugMun.Data
{
    internal abstract class TypedCaseFrame<TCase> : ICaseFrame where TCase : class
    {
        internal readonly Delegate IdGetter;
        internal readonly Delegate SolutionGetter;
        internal readonly Delegate[] Getters;
        internal readonly CaseDefinition Definition;

        public CaseSchema Schema { get; }

        public abstract int CaseCount { get; }

        protected TypedCaseFrame(CaseSchema schema)
        {
            Definition = new CaseDefinitionBuilder(typeof(TCase), schema).Build();
            Schema = schema ?? Definition.GetCaseSchema();
            IdGetter = DynamicCodeUtils.GenerateGetter<TCase>(Definition.IdAttribute);
            SolutionGetter = DynamicCodeUtils.GenerateGetter<TCase>(Definition.SolutionAttribute);
            Getters = GenerateGetters(Definition);
        }

        public abstract CaseCursor GetCaseCursor();

        private static Delegate[] GenerateGetters(CaseDefinition definition)
        {
            var result = new Delegate[definition.Count];
            for (var i = 0; i < definition.Count; i++)
            {
                result[i] = DynamicCodeUtils.GenerateGetter<TCase>(definition[i]);
            }

            return result;
        }
    }
}
