using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypescriptGenerator
{
    public class NamespaceCodeGenerator
    {
        private readonly string namespaceName;
        private readonly List<Type> types = new List<Type>();
        internal readonly AssemblyCodeGenerator codeGenerator;
        internal Dictionary<string, TypeCodeGenerator> codeGeneratorByType = new Dictionary<string, TypeCodeGenerator>();
        internal NamespaceCodeGenerator(string namespaceName, AssemblyCodeGenerator codeGenerator)
        {
            this.namespaceName = namespaceName;
            this.codeGenerator = codeGenerator;
        }
        internal void AddType(Type type)
        {
            types.Add(type);
            TypeCodeGenerator typeCodeGenerator;
            if (type.IsEnum)
            {
                typeCodeGenerator = new EnumCodeGenerator(type, this);
            }
            else
            {
                typeCodeGenerator = new InterfaceCodeGenerator(type, this);
            }
            codeGeneratorByType.Add(type.FullName, typeCodeGenerator);
        }

        internal void Write(StringBuilder sb, int levelOfIndentation)
        {
            if (codeGeneratorByType.Count > 0 && codeGeneratorByType.Select(x => x.Value).FirstOrDefault(x => x.CodeMustGenerated) != null)
            {
                WriteNamespaceStart(sb, levelOfIndentation);
                foreach (var typeCodeGenerator in codeGeneratorByType.OrderBy(t => t.Key).Select(o => o.Value))
                {
                    typeCodeGenerator.Write(sb, (levelOfIndentation + 1));
                }
                WriteNamespaceEnd(sb, levelOfIndentation);
                sb.AppendLine();
            }
        }

        private void WriteNamespaceStart(StringBuilder sb, int levelOfIndentation)
        {
            if (string.IsNullOrWhiteSpace(namespaceName) == false)
            {
                sb.AppendLine($"{Helpers.Indentation(levelOfIndentation)}declare namespace {this.namespaceName} {{");
            }
        }

        private void WriteNamespaceEnd(StringBuilder sb, int levelOfIndentation)
        {
            if (string.IsNullOrWhiteSpace(namespaceName) == false)
            {
                sb.AppendLine($"}}");
            }
        }

        private void WriteInterface(TypeCodeGenerator type, StringBuilder sb, int levelOfIndentation)
        {

        }

        List<string> generatedInterfaceName = new List<string>();

        internal void Compile()
        {
            foreach (var x in codeGeneratorByType)
            {
                x.Value.Compile();
            }
        }
    }
}
