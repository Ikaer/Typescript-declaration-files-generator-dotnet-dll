using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypescriptGeneratorCommons
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class TypescriptMorePropsAttribute : Attribute
    {
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public bool IsOptional { get; set; } 
        public TypescriptMorePropsAttribute(string propertyName, Type propertyType, bool isOptional = true)
        {
            PropertyName = propertyName;
            PropertyType = propertyType;
            IsOptional = isOptional;
        }
    }
}
