using System.Collections.Generic;
using System.Text;

namespace TypescriptGenerator
{
    public class Documentor
    {
        List<string> DocumentationLines = new List<string>();
        public void AddDocumentationLine(string line)
        {
            DocumentationLines.Add(line);
        }

        public void AddDocumentation(StringBuilder sb, int levelOfIndentation)
        {
            if (DocumentationLines.Count > 0)
            {
                sb.AppendLine($"{Helpers.Indentation(levelOfIndentation)}/**");
                foreach (string documentationLine in DocumentationLines)
                {
                    sb.AppendLine($"{Helpers.Indentation(levelOfIndentation)}* {documentationLine}");
                }
                sb.AppendLine($"{Helpers.Indentation(levelOfIndentation)}*/");
            }
        }
    }
}
