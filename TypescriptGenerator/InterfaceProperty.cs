using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypescriptGenerator
{
    public class InterfaceProperty : Documentor
    {
        public InterfaceProperty(string name, List<Type> types, bool isOptional = false)
        {
            Name = new PropertyName(name, this);
            Types = TypescriptTypeStore.AddTypes(types, true);
            foreach (var t in Types)
            {
                if (t.IsExcluded)
                {
                    AddDocumentationLine(t.DocumentationExcludedLine);
                }
            }
            IsOptional = isOptional;
            //AddDocumentationLine($"Original type: {Type.SourceType.Name}");
        }
        public InterfaceProperty(string name, Type type, bool isOptional = false) : this(name, new List<Type>() { type }, isOptional)
        {
        }
        public PropertyName Name { get; set; }
        public List<TypescriptType> Types { get; set; }
        public bool IsOptional { get; set; }

        public void Write(StringBuilder sb, string currentNamespace, int levelOfIndentation)
        {
            AddDocumentation(sb, levelOfIndentation);
            string strType = string.Join(" | ", Types.Select(t => t.QualifiedName(currentNamespace)).ToList());
            sb.AppendLine($"{Helpers.Indentation(levelOfIndentation)}{Name.Name}{(IsOptional ? "?" : "")}: {strType}");
        }
    }
}
