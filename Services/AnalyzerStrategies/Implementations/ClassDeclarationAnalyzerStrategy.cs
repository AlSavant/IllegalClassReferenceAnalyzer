using IllegalClassReferenceAnalyzer.Services.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;

namespace IllegalClassReferenceAnalyzer.Services.AnalyzerStrategies.Implementations
{
    internal sealed class ClassDeclarationAnalyzerStrategy : IAnalyzerStrategy
    {
        public void AnalyzeNode(SyntaxNodeAnalysisContext context, DiagnosticDescriptor rule, HashSet<string> forbiddenTypeNames, HashSet<string> allowedTypeNames)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;            

            if(classDeclaration.BaseList != null)
            {
                foreach (var parameter in classDeclaration.BaseList.Types)
                {
                    var parameterType = context.SemanticModel.GetTypeInfo(parameter.Type).Type;
                    if (parameterType.IsForbiddenType(forbiddenTypeNames, allowedTypeNames))
                    {
                        var diagnostic = Diagnostic.Create(rule, parameter.GetLocation(), parameterType);
                        context.ReportDiagnostic(diagnostic);
                        return;
                    }
                }
            }

            if(classDeclaration.AttributeLists != null)
            {
                foreach (var attributeList in classDeclaration.AttributeLists)
                {
                    foreach(var attribute in attributeList.Attributes)
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
