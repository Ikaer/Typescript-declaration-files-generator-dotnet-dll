using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypescriptGeneratorCommons
{
    [AttributeUsage(AttributeTargets.All)]
    public class TypescriptOptionalAttribute : Attribute
    {
        public TypescriptOptionalAttribute()
        {

        }
    }
}
