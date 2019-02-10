using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TypescriptGenerator
{
    public class EnumCodeGenerator : TypeCodeGenerator
    {
        internal EnumCodeGenerator(Type type, NamespaceCodeGenerator codeGeneratorByNamespace) : base(type, codeGeneratorByNamespace)
        {
        }

        internal List<string> EnumValues { get; set; } = new List<string>();

        internal override void Write(StringBuilder sb, int levelOfIndentation)
        {
            try
            {
                var typeInfo = type.GetTypeInfo();
                var className = type.Name;
                List<string> enumValues = new List<string>();
                foreach (var value in Enum.GetValues(type))
                {
                    enumValues.Add(value.ToString());
                }
                if (enumValues.Count > 0)
                {
                    sb.AppendLine($"{Helpers.Indentation(levelOfIndentation)}type {this.TypescriptType.Type} =");


                    for (var i = 0; i < enumValues.Count; i++)
                    {
                        sb.AppendLine($"{Helpers.Indentation(levelOfIndentation)}{(i > 0 ? "| " : "")}'{enumValues[i]}'{(i == enumValues.Count - 1 ? ";" : "")}");
                    }
                }
                sb.AppendLine("");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] {e.Message}");
            }
        }
        
        internal override void Compile()
        {
            try
            {
                List<string> enumValues = new List<string>();
                foreach (var value in Enum.GetValues(type))
                {
                    EnumValues.Add(value.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] {e.Message}");
            }
        }

        internal override bool IsEmpty()
        {
            return EnumValues.Count == 0;
        }
    }
}
