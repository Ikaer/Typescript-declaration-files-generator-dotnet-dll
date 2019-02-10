using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TypescriptGenerator
{
    public class AssemblyCodeGenerator
    {
        public Dictionary<string, NamespaceCodeGenerator> codeGeneratorByNamespace = new Dictionary<string, NamespaceCodeGenerator>();
        private readonly AssemblyAndTranspilationOptions assembly;
        private readonly CodeGenerator codeGenerator;

        public AssemblyCodeGenerator(AssemblyAndTranspilationOptions assembly, CodeGenerator codeGenerator)
        {
            this.assembly = assembly;
            this.codeGenerator = codeGenerator;
            var ToProcessCount = assembly.Assembly.DefinedTypes.Count();
            var ProcessedCount = assembly.Assembly.DefinedTypes.Count();
            Console.WriteLine($"{ToProcessCount} type to process");

            foreach (var type in assembly.Assembly.DefinedTypes)
            {
                if (type.Name != "<PrivateImplementationDetails>" && type.IsNestedPrivate == false)
                {
                    try
                    {
                        bool canTypeBeAdded = true;
                        var ns = type.Namespace;
                        if (type.Namespace == null)
                        {
                            canTypeBeAdded = false;
                        }
                        else if (assembly.Options != null && assembly.Options.FilterNamespace != null && assembly.Options.FilterNamespace.Count > 0)
                        {
                            if (assembly.Options.FilterNamespace.Find(fn => ns.StartsWith(fn)) == null)
                            {
                                canTypeBeAdded = false;
                            }
                            else
                            {
                                canTypeBeAdded = true;
                            }
                        }
                        // todo: trouver d'ou viennent les classes foireuses.
                        if (canTypeBeAdded)
                        {
                            AddType(type);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[ERROR] Namespace: {(type.Namespace == null ? "<none>" : type.Namespace)}, class: {type.Name}");
                        Console.WriteLine($"{e.GetType().Name}:: {e.Message}");
                        Console.WriteLine($"{e.StackTrace}");
                        ProcessedCount--;
                    }
                }
            }
        }

        private void AddType(Type type)
        {
            NamespaceCodeGenerator _codeGeneratorByNamespace;
            string ns = type.Namespace;
            if (ns == null)
            {
                ns = "";
            }
            if (codeGeneratorByNamespace.TryGetValue(ns, out _codeGeneratorByNamespace) == false)
            {
                _codeGeneratorByNamespace = new NamespaceCodeGenerator(ns, this);
                codeGeneratorByNamespace.Add(ns, _codeGeneratorByNamespace);
            }

            _codeGeneratorByNamespace.AddType(type);
        }

        public string GenerateCode()
        {
            var name = assembly.Assembly.GetName().Name;
            StringBuilder sb = new StringBuilder();
            foreach (var x in codeGeneratorByNamespace.OrderBy(k => k.Key))
            {
                x.Value.Write(sb, 0);
            }
            return sb.ToString();
        }


        internal void Compile()
        {
            foreach (var x in codeGeneratorByNamespace)
            {
                x.Value.Compile();
            }
        }
    }
}
