# Typescript declaration files generator from .Net dlls.
Generator of declaration file in Typescript from .Net dlls 

This is a simple project that takes one or more .Net dlls as input and an ouput folder and generates a declaration file for each one of those dll.

This solution:
* Keeps namespaces in declaration files
* Converts "classic" types of .Net into Typescript types.
* Recursively browses types to add them to declaration files.
* Can exclude namespaces from generation.
* Uses "any" for specified excluded types.
* Transpiles generic types the correct way.
* Handle inheritance.

Limitations:
* .Net nested classes are generated but not inside classes declaration on Typescript side (it does not exist).
* Methods are not generated at the moment (but plan to develop it if the need occurs)
* When a class is declared as a non generic version and a generic version in c# code, a class with suffix 1 will be generated because Typescript does not support same class name even with a difference in genericity. For example classes Shape and Shape<T> inside the same namespace will generate class Shape and class Shape1<T>.

The core projects are "TypescriptGenerator" and "TypescriptGeneratorCommons". "csaTest" and "TestLibrary" are just here to demonstrate the solution.

This project has been developed, among other things:
* to create proxys between server objects and a client to avoid property or classe names/types breaking changes.
* To get a list of server returned types without write it down by hand.

# How to test the console application
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

classX.cs
```C#
namespace TestLibrary
{
    public class ClassX<T>
    {
        public T AGenericProperty { get; set; }
    }
}
```

class0.cs
```C#
using TypescriptGeneratorCommons;

namespace TestLibrary
{
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
    [TypescriptMoreProps("AnotherProp", typeof(Class0), true)]
    public class Class1
    {
        public int MyProp_Int { get; set; }

        public string MyProp_String { get; set; }

        public DateTime MyProp_DateTime { get; set; }

        public TimeSpan MyProp_Timespan { get; set; }

        [TypescriptOptional]
        public Guid MyProp_Guid { get; set; }
	
	public ClassX<Class0> MyGenericPropertyWithAnArgument { get; set; }
    }
}
```

File generated: TestLibrary.d.ts
```Typescript
declare namespace TestLibrary {
	interface Class0 {
		PropX?: number | string | boolean
		PropY?: Class0
	}

	interface Class1 {
		MyGenericPropertyWithAnArgument: ClassX<Class0>
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

	interface ClassX<T> {
		AGenericProperty: T
	}

}


```

# Class and property decorators

Some class and property attributes can help you pilot the transpilation.

In order to do that, you must add the project "TypescriptGeneratorCommons" to your projects.

* TypescriptMoreProps:
To add properties that are not in server side. For example, in javascript you can add property on a object you've get from the server even if this property was not here initialy. This attribute help you to do that.

```C#
[TypescriptMoreProps("PropertyOnlyOnClientSide", typeof(string), true)]
public class MyClass {}
```

* TypescriptOptional
Can be placed on both classes and properties. This attribute make the properties optional with the question mark in Typescript. It can be useful, because some times, you create new objects which are shaped like the server one, but miss some properties that will be added later.

* TypescriptMoreType
Can be used to handle type intersection in Typescript. We cannot do that in c#, but because we can in javascript, this attribute allows you to add extra types to your properties.

For example:
```C#
[TypescriptMoreType(typeof(string), typeof(bool))]
public int PropX { get; set; }
```
will generate
```Typescript
PropX: number | string | boolean
```




