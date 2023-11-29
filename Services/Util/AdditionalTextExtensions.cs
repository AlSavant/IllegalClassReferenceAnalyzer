using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace IllegalClassReferenceAnalyzer.Services.Util
{
    internal static class AdditionalTextExtensions
    {
        public static HashSet<string> ParseForbiddenTypeNames(this AdditionalText additionalText, HashSet<string> forbiddenTypeNames)
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
            catch { }
            return forbiddenTypeNames;
        }
    }
}
