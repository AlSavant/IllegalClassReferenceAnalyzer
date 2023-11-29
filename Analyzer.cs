using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using IllegalClassReferenceAnalyzer.Services.AnalyzerStrategies;
using IllegalClassReferenceAnalyzer.Services.AnalyzerStrategies.Implementations;
using IllegalClassReferenceAnalyzer.Services.Util;
using System.Linq;

namespace IllegalClassReferenceAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class Analyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "WOD001";
        private const string Category = "Security";
        private static readonly DiagnosticDescriptor rule = new DiagnosticDescriptor(
            DiagnosticId,
            "Illegal Type reference detected",
            "Type {0} should not be referenced for security reasons. Use alternative types provided in internal libraries instead.",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(rule); } }
        private readonly Dictionary<SyntaxKind, IAnalyzerStrategy> strategies = new Dictionary<SyntaxKind, IAnalyzerStrategy>();
        private readonly SyntaxKind[] registeredKinds;

        public Analyzer()
        {
            strategies.Add(SyntaxKind.ConstructorDeclaration, new ConstructorDeclarationAnalyzerStrategy());
            strategies.Add(SyntaxKind.VariableDeclaration, new VariableDeclarationAnalyzerStrategy());
            strategies.Add(SyntaxKind.MethodDeclaration, new MethodDeclarationAnalyzerStrategy());
            strategies.Add(SyntaxKind.PropertyDeclaration, new PropertyDeclarationAnalyzerStrategy());
            strategies.Add(SyntaxKind.ClassDeclaration, new ClassDeclarationAnalyzerStrategy());
            strategies.Add(SyntaxKind.InterfaceDeclaration, new InterfaceDeclarationAnalyzerStrategy());

            registeredKinds = strategies.Keys.ToArray();
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, registeredKinds);                
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {               
            var forbiddenTypeNames = new HashSet<string>();
            foreach(var file in context.Options.AdditionalFiles)
            {
                file.ParseForbiddenTypeNames(forbiddenTypeNames);
            }
            var syntaxKind = context.Node.Kind();
            if(strategies.ContainsKey(syntaxKind))
            {
                strategies[syntaxKind].AnalyzeNode(context, rule, forbiddenTypeNames);
            }            
        }                      
    }
}
