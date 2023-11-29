using IllegalClassReferenceAnalyzer.Services.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;

namespace IllegalClassReferenceAnalyzer.Services.AnalyzerStrategies.Implementations
{
    internal sealed class MethodDeclarationAnalyzerStrategy : IAnalyzerStrategy
    {
        public void AnalyzeNode(SyntaxNodeAnalysisContext context, DiagnosticDescriptor rule, HashSet<string> forbiddenTypeNames)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            if (methodDeclaration.ReturnType == null)
            {
                return;
            }
            var returnType = context.SemanticModel.GetTypeInfo(methodDeclaration.ReturnType).Type;
            if (returnType.IsForbiddenType(forbiddenTypeNames))
            {
                var diagnostic = Diagnostic.Create(rule, methodDeclaration.GetLocation(), returnType);
                context.ReportDiagnostic(diagnostic);
                return;
            }
            foreach (var parameter in methodDeclaration.ParameterList.Parameters)
            {
                var parameterType = context.SemanticModel.GetTypeInfo(parameter.Type).Type;
                if (parameterType.IsForbiddenType(forbiddenTypeNames))
                {
                    var diagnostic = Diagnostic.Create(rule, parameter.GetLocation(), parameterType);
                    context.ReportDiagnostic(diagnostic);
                    return;
                }
            }

            if(methodDeclaration.AttributeLists != null)
            {
                foreach (var attributeList in methodDeclaration.AttributeLists)
                {
                    foreach (var attribute in attributeList.Attributes)
                    {
                        var attributeType = context.SemanticModel.GetTypeInfo(attribute).Type;
                        if (attributeType.IsForbiddenType(forbiddenTypeNames))
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
