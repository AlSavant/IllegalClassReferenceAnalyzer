using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace IllegalClassReferenceAnalyzer.Services.Util
{
    internal static class TypeSymbolExtensions
    {
        public static bool IsForbiddenType(this ITypeSymbol typeSymbol, HashSet<string> forbiddenTypeNames)
        {
            if(typeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType)
            {
                bool foundMatch = false;
                foreach(var typeArgument in namedTypeSymbol.TypeArguments)
                {
                    if(IsForbiddenType(typeArgument, forbiddenTypeNames))
                    {
                        foundMatch = true;
                        break;
                    }
                }
                return foundMatch;
            }
            if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
            {
                return IsForbiddenType(arrayTypeSymbol.ElementType, forbiddenTypeNames);                
            }
            var typeString = $"{typeSymbol.ContainingNamespace}.{typeSymbol.Name}";
            return forbiddenTypeNames.Contains(typeString);
        }        
    }
}
