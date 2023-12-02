using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace IllegalClassReferenceAnalyzer.Services.Util
{
    internal static class TypeSymbolExtensions
    {
        public static bool IsForbiddenType(this ITypeSymbol typeSymbol, HashSet<string> forbiddenTypeNames, HashSet<string> allowedTypeNames)
        {
            if(typeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType)
            {
                bool foundMatch = false;
                foreach(var typeArgument in namedTypeSymbol.TypeArguments)
                {
                    if(IsForbiddenType(typeArgument, forbiddenTypeNames, allowedTypeNames))
                    {
                        foundMatch = true;
                        break;
                    }
                }
                return foundMatch;
            }
            if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
            {
                return IsForbiddenType(arrayTypeSymbol.ElementType, forbiddenTypeNames, allowedTypeNames);                
            }
            var typeString = $"{typeSymbol.ContainingNamespace}.{typeSymbol.Name}";
            return forbiddenTypeNames.Contains(typeString) && !allowedTypeNames.Contains(typeString);
        }        
    }
}
