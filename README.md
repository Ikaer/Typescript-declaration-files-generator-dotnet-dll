# TypescriptGenerator
Generator of declaration file in Typescript from .Net dlls 

This is a simple project that takes one or more .Net dlls as input and an ouput folder and generates a declaration file for each one of those dll.

This solution:
* Keep namespaces in declaration files
* Convert "classic" types of .Net into Typescript types.
* Recursively browse types to add them to declaration files.
* Can exclude namespaces from generation.
* Use "any" for specified excluded types.

The core projects are "TypescriptGenerator" and "TypescriptGeneratorCommons". "csaTest" and "TestLibrary" are just here to demonstrate the solution.

This project has been developed, among other things:
* to create proxys between server objects and a client to avoid property or classe names/types breaking changes.
* To get a list of server returned types without write it down by hand.


To test the console application, just change the following command:
```
"C:\Program Files\dotnet\dotnet.exe" exec "X:Path\To\TypescriptFilesGenerator\csaTest\bin\Debug\netcoreapp2.2\csaTest.dll" "X:Path\To\TypescriptFilesGenerator\csaTest\ouput" "X:Path\To\TypescriptFilesGenerator\csaTest\dlls\TestLibrary.dll"
```
with you directories.

The arguments are 
* first one is always the output directory for generated declaration files.
* all arguments after the first one are paths to Dll.


# Example
An example of generated files from those classes
class0.cs
```C#
using TypescriptGeneratorCommons;

namespace TestLibrary
{
    /// <summary>
    /// idid
    /// </summary>
    [TypescriptOptional]
    public class Class0
    {
        [TypescriptMoreType(typeof(string))]
        public bool PropX { get; set; }

        public Class0 PropY { get; set; }
    }
}

```

class1.cs
```C#
using System;
using TypescriptGeneratorCommons;

namespace TestLibrary
{

    /// <summary>
    /// iriri
    /// </summary>
    [TypescriptMoreProps("AnotherProp", typeof(Class0), true)]
    public class Class1
    {
        public int MyProp_Int { get; set; }

        /// <summary>
        /// With some documentation
        /// </summary>
        public string MyProp_String { get; set; }

        public DateTime MyProp_DateTime { get; set; }

        public TimeSpan MyProp_Timespan { get; set; }

        [TypescriptOptional]
        public Guid MyProp_Guid { get; set; }
    }
}
```

File generated: TestLibrary.d.ts
```Typescript
declare namespace TestLibrary {
	interface Class0 {
		PropX?: boolean | string
		PropY?: Class0
	}

	interface Class1 {
		MyProp_DateTime: string
		MyProp_Guid?: string
		MyProp_Int: number
		MyProp_String: string
		/**
		* any: because System.TimeSpan has been exclude
		*/
		MyProp_Timespan: any
		AnotherProp?: Class0
	}

}
```
