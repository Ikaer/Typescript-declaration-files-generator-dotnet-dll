using System.Collections.Generic;
using System.Reflection;

namespace TypescriptGenerator
{
    public class CodeGenerator
    {
        internal Dictionary<string, AssemblyCodeGenerator> CodeGeneratorByAssembly = new Dictionary<string, AssemblyCodeGenerator>();

        public CodeGenerator(List<string> excludedNamespaces, List<string> excludedTypes)
        {
            TypescriptTypeStore.LoadExclusionRules(excludedNamespaces, excludedTypes);
        }

        public void AddAssembly(AssemblyAndTranspilationOptions assembly)
        {
            CodeGeneratorByAssembly.Add(assembly.Assembly.GetName().Name, new AssemblyCodeGenerator(assembly, this));

        }

        internal void Compile()
        {
            foreach (var codeGeneratorByAssembly in CodeGeneratorByAssembly)
            {
                codeGeneratorByAssembly.Value.Compile();
            }
        }


    }
}
