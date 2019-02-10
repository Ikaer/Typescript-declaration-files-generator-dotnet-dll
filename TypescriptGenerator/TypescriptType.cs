using System;

namespace TypescriptGenerator
{
    public class TypescriptType
    {
        public TypescriptType(Type type, bool isExcluded)
        {
            SourceType = type;
            IsExcluded = isExcluded;
            if (IsExcluded)
            {
                DocumentationExcludedLine = $"any: because {type.FullName} has been exclude";
            }
        }
        public string Type { get; set; }
        public string NamespaceName { get; set; }
        public string QualifiedName(string currentNamespace)
        {
            if (IsExcluded)
            {
                return "any";
            }
            if (this.NamespaceName == currentNamespace || string.IsNullOrWhiteSpace(NamespaceName) || AlwaysExcludeNamespace == true)
            {
                return Type;
            }
            else
            {
                return $"{NamespaceName}.{Type}";
            }
        }
        public Type SourceType { get; }
        public bool IsExcluded { get; }
        public string DocumentationExcludedLine { get; private set; }
        public bool AlwaysExcludeNamespace { get; set; }
        public bool TouchedInProperty { get; set; }
    }
}
