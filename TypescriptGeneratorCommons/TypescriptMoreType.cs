using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypescriptGeneratorCommons
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TypescriptMoreType : Attribute
    {
        public List<Type> PropertyTypes { get; set; }

        public TypescriptMoreType(params Type[] types)
        {
            this.PropertyTypes = types != null ? types.ToList() : new List<Type>();
        }
        public TypescriptMoreType(List<Type> types)
        {
            this.PropertyTypes = types != null ? types : new List<Type>();
        }
    }
}
