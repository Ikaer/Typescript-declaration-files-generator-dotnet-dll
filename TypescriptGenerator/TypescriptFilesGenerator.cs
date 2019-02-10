using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TypescriptGenerator
{
    public class TranspilationOptions
    {
        public List<string> FilterNamespace;
    }

    public class AssemblyAndTranspilationOptions
    {
        public Assembly Assembly;
        public TranspilationOptions Options;
    }

    public class TypescriptFilesGenerator
    {
        private List<AssemblyAndTranspilationOptions> assemblies;
        private string outputPath;
        private List<string> excludedNamespaces;
        private List<string> excludedTypes;
        private Action<string> actionWhenFileModified;
        public TypescriptFilesGenerator(List<AssemblyAndTranspilationOptions> assemblies, string outputPath, List<string> excludedNamespaces, List<string> excludedTypes, 
            Action<string> actionWhenFileModified)
        {

            if (assemblies == null || assemblies.Count == 0)
            {
                Console.WriteLine("assemblies is null or empty.");
                return;
            }
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                Console.WriteLine("outputPath is null or empty.");
            }
            else
            {
                this.assemblies = assemblies;
                this.outputPath = outputPath;
                this.excludedNamespaces = excludedNamespaces == null ? new List<string>() : excludedNamespaces;
                this.excludedTypes = excludedTypes == null ? new List<string>() : excludedTypes;
                this.actionWhenFileModified = actionWhenFileModified;
                Transform();
            }
        }
        private void Transform()
        {
            CodeGenerator codeGenerator = new CodeGenerator(this.excludedNamespaces, this.excludedTypes);
            foreach (var assembly in assemblies)
            {
                codeGenerator.AddAssembly(assembly);
            }
            codeGenerator.Compile();
            foreach(var codeGeneratoryByAssembly in codeGenerator.CodeGeneratorByAssembly)
            {
                string assemblyName = codeGeneratoryByAssembly.Key;
                string assemblyCode = codeGeneratoryByAssembly.Value.GenerateCode();
                
                string typescriptFilePath = Path.Combine(this.outputPath, $"{assemblyName}.d.ts");

                string previousAssemblyCode = null;
                bool fileExists = File.Exists(typescriptFilePath);
                if (fileExists)
                {
                    previousAssemblyCode = File.ReadAllText(typescriptFilePath);
                }

                bool mustReadonly = false;
                if (previousAssemblyCode == null || previousAssemblyCode != assemblyCode)
                {
                    if (fileExists)
                    {
                        FileAttributes attributes = File.GetAttributes(typescriptFilePath);

                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            mustReadonly = true;
                            attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
                            File.SetAttributes(typescriptFilePath, attributes);
                            Console.WriteLine($"Remove readonly on {typescriptFilePath}.");
                        }
                    }

                    Console.WriteLine($"Make change in {typescriptFilePath} file.");
                    File.WriteAllText(typescriptFilePath, assemblyCode);

                    actionWhenFileModified?.Invoke(typescriptFilePath);

                    if (mustReadonly)
                    {
                        File.SetAttributes(typescriptFilePath, File.GetAttributes(typescriptFilePath) | FileAttributes.ReadOnly);
                        Console.WriteLine("The {0} file is now readonly again.", typescriptFilePath);
                    }
                }
                else
                {
                    Console.WriteLine($"No change made in {typescriptFilePath} file.");
                }
            }
        }

        private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }

        private string writeNamespaceFolders(string previousPath, string currentPart)
        {
            var pathToCreateIfNotExists = Path.Combine(previousPath, currentPart);
            Directory.CreateDirectory(pathToCreateIfNotExists);
            return pathToCreateIfNotExists;
        }
    }
}
