using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using IllegalClassReferenceAnalyzer.Services.Util;

namespace IllegalClassReferenceAnalyzer.Services.AnalyzerStrategies.Implementations
{
    internal sealed class PropertyDeclarationAnalyzerStrategy : IAnalyzerStrategy
    {
        public void AnalyzeNode(SyntaxNodeAnalysisContext context, DiagnosticDescriptor rule, HashSet<string> forbiddenTypeNames, HashSet<string> allowedTypeNames)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;
            if (propertyDeclaration.Type == null)
            {
                return;
            }
            var variableType = context.SemanticModel.GetTypeInfo(propertyDeclaration.Type).Type;
            if (variableType.IsForbiddenType(forbiddenTypeNames, allowedTypeNames))
            {
                var diagnostic = Diagnostic.Create(rule, propertyDeclaration.GetLocation(), variableType);
                context.ReportDiagnostic(diagnostic);
                return;
            }

            if (propertyDeclaration.AttributeLists != null)
            {
                foreach (var attributeList in propertyDeclaration.AttributeLists)
                {
                    foreach (var attribute in attributeList.Attributes)
                    {
                        var attributeType = context.SemanticModel.GetTypeInfo(attribute).Type;
                        if (attributeType.IsForbiddenType(forbiddenTypeNames, allowedTypeNames))
                        {
                            var diagnostic = Diagnostic.Create(rule, attribute.GetLocation(), attributeType);
                            context.ReportDiagnostic(diagnostic);
                            return;
                        }
                    }
                }
            }
        }
    }
}
