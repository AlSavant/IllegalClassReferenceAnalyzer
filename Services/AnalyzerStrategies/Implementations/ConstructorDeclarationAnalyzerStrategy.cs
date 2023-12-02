using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using IllegalClassReferenceAnalyzer.Services.Util;

namespace IllegalClassReferenceAnalyzer.Services.AnalyzerStrategies.Implementations
{
    internal sealed class ConstructorDeclarationAnalyzerStrategy : IAnalyzerStrategy
    {
        public void AnalyzeNode(SyntaxNodeAnalysisContext context, DiagnosticDescriptor rule, HashSet<string> forbiddenTypeNames, HashSet<string> allowedTypeNames)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;
            foreach (var parameter in constructorDeclaration.ParameterList.Parameters)
            {
                var parameterType = context.SemanticModel.GetTypeInfo(parameter.Type).Type;
                if (parameterType.IsForbiddenType(forbiddenTypeNames, allowedTypeNames))
                {
                    var diagnostic = Diagnostic.Create(rule, parameter.GetLocation(), parameterType);
                    context.ReportDiagnostic(diagnostic);
                    return;
                }
            }
            if (constructorDeclaration.AttributeLists != null)
            {
                foreach (var attributeList in constructorDeclaration.AttributeLists)
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
