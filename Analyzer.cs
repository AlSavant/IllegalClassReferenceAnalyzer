using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml.Linq;
using System.Reflection;
using System.IO;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace IllegalClassReferenceAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class Analyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "WOD001";
        private const string Category = "IllegalInjection";

        private HashSet<string> forbiddenTypeNames = new HashSet<string>();

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            "Illegal Injection detected",
            "Type {0} should not be injected in service for security reasons. Use alternative types provided in internal libraries instead.",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {            
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeConstructor, SyntaxKind.ConstructorDeclaration);
        }

        private void AnalyzeConstructor(SyntaxNodeAnalysisContext context)
        {
            foreach(var file in context.Options.AdditionalFiles)
            {
                ReadSettingsFromXml(file);
            }
            
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            foreach (var parameter in constructorDeclaration.ParameterList.Parameters)
            {
                var parameterType = context.SemanticModel.GetTypeInfo(parameter.Type).Type;
                               
                if (IsForbiddenType(parameterType))
                {
                    var diagnostic = Diagnostic.Create(Rule, parameter.GetLocation(), parameterType);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }        

        private bool IsForbiddenType(ITypeSymbol typeSymbol)
        {            
            var typeString = $"{typeSymbol.ContainingNamespace}.{typeSymbol.Name}";
            return forbiddenTypeNames.Contains(typeString);
        }       

        private void ReadSettingsFromXml(AdditionalText additionalText)
        {
            try
            {
                SourceText fileText = additionalText.GetText();
                MemoryStream stream = new MemoryStream();
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                {
                    fileText.Write(writer);
                }

                stream.Position = 0;
                XDocument doc = XDocument.Load(stream);
                
                var rootNode = doc.Element("AnalyzerSettings");
                var elements = rootNode?.Elements("ForbiddenType");
                if (elements != null)
                {
                    foreach (var element in elements)
                    {
                        if (!string.IsNullOrEmpty(element.Value))
                        {
                            forbiddenTypeNames.Add(element.Value);
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }
    }
}
