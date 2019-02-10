using System;
using System.Collections.Generic;
using System.Reflection;
using TypescriptGenerator;

namespace csaTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"====================== Typescript generator ====================");
            List<string> dllPaths = new List<string>();
            string outputPath = "";
            for (var i = 0; i < args.Length; i++)
            {
                if (i == 0)
                {
                    outputPath = args[i];
                    Console.WriteLine($"Output folder: {args[i]}");
                }
                else
                {
                    dllPaths.Add(args[i]);
                    Console.WriteLine($"Dll to use: {args[i]}");
                }
            }

            List<string> excludedNamespace = new List<string>()
            {
                "System.Data",
                "Newtonsoft",
                "System.Xml",
                "System.Runtime",
                "System.Uri",
                "System.Reflection",
                "System.ServiceModel",
                "Microsoft.AspNet.SignalR",
                "System.ComponentModel",
                "System.Globalization",
                "Dont.Want.This.Namespace.Either"
            };
            List<string> excludedTypes = new List<string>()
            {
                "System.Type",
                "System.Byte",
                "System.TimeSpan",
                "System.Version",
                "System.Exception",
                "System.Collections.IDictionary",
                "System.Collections.Generic.IEqualityComparer*",
                "System.Collections.Generic*KeyCollection*",
                "System.Collections.Generic*ValueCollection*",
                "Dont.Want.This.Type.Either"
            };
            List<AssemblyAndTranspilationOptions> assemplies = new List<AssemblyAndTranspilationOptions>();
            foreach (string dllPath in dllPaths)
            {
                Console.WriteLine($"Loading assembly from {dllPath}");
                var transpilationOptions = new TranspilationOptions();
                if (dllPath.EndsWith("Dll.To.Filter.dll"))
                {
                    transpilationOptions.FilterNamespace = new List<string>() { "Only.This.Namespace" };
                }

                assemplies.Add(new AssemblyAndTranspilationOptions() { Assembly = Assembly.LoadFrom(dllPath), Options = transpilationOptions });

            }
            var generator = new TypescriptFilesGenerator(assemplies, outputPath, excludedNamespace, excludedTypes, (filePath) =>
            {
                // this callback is used when a file has changed.
                // You can use it for example to automatically check out a file on a source control
            });
            Console.ReadLine();
        }
    }
}
