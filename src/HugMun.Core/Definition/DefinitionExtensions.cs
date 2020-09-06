using System;

namespace HugMun.Core
{
    public static class DefinitionExtensions
    {
        public static CaseSchema GetCaseSchema(this CaseDefinition caseDefinition)
        {
            if(caseDefinition == null) throw new ArgumentNullException(nameof(caseDefinition));

            var builder = new CaseSchemaBuilder()
                .AddId(caseDefinition.IdAttribute.Name)
                .AddSolution(caseDefinition.SolutionAttribute.Name, caseDefinition.SolutionAttribute.Type);

            foreach (var attribute in caseDefinition)
            {
                builder.AddAttribute(attribute.Name, attribute.Type);
            }

            return builder.Build();
        }
    }
}
