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

        public ClassX<Class0> MyGenericPropertyWithAnArgument { get; set; }
    }
}
