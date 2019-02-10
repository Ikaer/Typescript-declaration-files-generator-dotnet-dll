using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TypescriptGeneratorCommons;

namespace TypescriptGenerator
{
    public class InterfaceCodeGenerator : TypeCodeGenerator
    {
        internal InterfaceCodeGenerator(Type type, NamespaceCodeGenerator codeGeneratorByNamespace) : base(type, codeGeneratorByNamespace)
        {

        }

        internal List<InterfaceProperty> InterfaceProperties { get; set; } = new List<InterfaceProperty>();

        internal override void Write(StringBuilder sb, int levelOfIndentation)
        {
            try
            {
                if (this.TypescriptType.TouchedInProperty == false && InterfaceProperties.Count == 0)
                {
                    //Console.WriteLine($"[{this.type.FullName}] is empty and not used anywhere: code will not be generated.");
                }
                else
                {
                    sb.AppendLine($"{Helpers.Indentation(levelOfIndentation)}interface {this.TypescriptType.Type} {{");
                    foreach (var property in InterfaceProperties)
                    {
                        foreach (var t in property.Types)
                        {
                            if (t.IsExcluded)
                            {
                                AddDocumentationLine(t.DocumentationExcludedLine);
                            }
                        }
                        property.Write(sb, this.TypescriptType.NamespaceName, (levelOfIndentation + 1));
                    }
                    sb.AppendLine($"{Helpers.Indentation(levelOfIndentation)}}}");
                    sb.AppendLine("");
                }
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
                string typeName = type.Name;
                if (typeName == "TargetGroupExploit")
                {

                }
                var allOptional = type.GetCustomAttribute<TypescriptOptionalAttribute>() != null;
                foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public).OrderBy(p => p.Name))
                {
                    var specificOptional = property.GetCustomAttribute<TypescriptOptionalAttribute>() != null;
                    var otherTypes = property.GetCustomAttribute<TypescriptMoreType>();
                    var types = new List<Type>();
                    types.Add(property.PropertyType);
                    if (otherTypes != null)
                    {
                        types.AddRange(otherTypes.PropertyTypes);
                    }

                    InterfaceProperties.Add(new InterfaceProperty(property.Name, types, allOptional || specificOptional));
                }

                var moreProps = type.GetCustomAttributes<TypescriptMorePropsAttribute>().ToList();
                if (moreProps != null && moreProps.Count > 0)
                {
                    moreProps.ForEach(mp =>
                    {
                        InterfaceProperties.Add(new InterfaceProperty(mp.PropertyName, mp.PropertyType, mp.IsOptional));
                    });
                }
            }
            catch (Exception e)
            {

            }
        }

        internal override bool IsEmpty()
        {
            return InterfaceProperties.Count == 0;
        }
    }
}
