using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;

namespace IllegalClassReferenceAnalyzer.Services.AnalyzerStrategies
{
    internal interface IAnalyzerStrategy
    {
        void AnalyzeNode(SyntaxNodeAnalysisContext context, DiagnosticDescriptor rule, HashSet<string> forbiddenTypeNames);
    }
}
