using IllegalClassReferenceAnalyzer.Services.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;

namespace IllegalClassReferenceAnalyzer.Services.AnalyzerStrategies.Implementations
{
    internal sealed class VariableDeclarationAnalyzerStrategy : IAnalyzerStrategy
    {
        public void AnalyzeNode(SyntaxNodeAnalysisContext context, DiagnosticDescriptor rule, HashSet<string> forbiddenTypeNames)
        {
            var variableDeclaration = (VariableDeclarationSyntax)context.Node;
            if(variableDeclaration.Type == null)
            {
                return;
            }
            var variableType = context.SemanticModel.GetTypeInfo(variableDeclaration.Type).Type;
            if (variableType.IsForbiddenType(forbiddenTypeNames))
            {
                var diagnostic = Diagnostic.Create(rule, variableDeclaration.GetLocation(), variableType);
                context.ReportDiagnostic(diagnostic);
                return;
            }            
        }
    }
}
