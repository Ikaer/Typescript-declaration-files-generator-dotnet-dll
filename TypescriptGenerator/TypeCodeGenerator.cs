using System;
using System.Text;

namespace TypescriptGenerator
{
    public abstract class TypeCodeGenerator : Documentor
    {
        internal TypescriptType TypescriptType;
        protected readonly Type type;
        private readonly NamespaceCodeGenerator namespaceCodeGenerator;

        internal TypeCodeGenerator(Type type, NamespaceCodeGenerator namespaceCodeGenerator)
        {
            this.type = type;
            this.namespaceCodeGenerator = namespaceCodeGenerator;
            TypescriptType = TypescriptTypeStore.AddType(type, false);
        }

        internal abstract void Compile();

        internal abstract void Write(StringBuilder sb, int levelOfIndentation);
        
        internal abstract bool IsEmpty();

        internal bool IsTouchedByProperty
        {
            get
            {
                return TypescriptType.TouchedInProperty;
            }
        }

        internal bool CodeMustGenerated
        {
            get
            {
                return IsEmpty() == false || IsTouchedByProperty;
            }
        }
    }
}
