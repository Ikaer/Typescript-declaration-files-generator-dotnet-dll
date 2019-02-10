using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TypescriptGeneratorCommons;

namespace TypescriptGenerator
{
    public class TypescriptTypeStore
    {
        /// <summary>
        /// Key is namespace + non-generic name (for example MyClass`2 become MyClass.
        /// Because Typescript cannot handle in the same namespace a generic and non-generic classes with the same name.
        /// </summary>
        private static Dictionary<string, int> IncrementByName = new Dictionary<string, int>();
        /// <summary>
        /// List of excluded namespaces, test is done on type namespace with a startsWith.
        /// </summary>
        private static List<string> excludedNamespaces;
        /// <summary>
        /// List of excluded fully qualified type names.
        /// </summary>
        private static List<string> excludedTypes;

        private static Dictionary<Type, TypescriptType> store = new Dictionary<Type, TypescriptType>();


        public static List<TypescriptType> AddTypes(List<Type> types, bool fromProperty)
        {
            List<TypescriptType> ret = new List<TypescriptType>();
            if (types != null)
            {
                foreach(var type in types)
                {
                    var typescriptType = AddType(type, fromProperty);
                    if (typescriptType != null && ret.Contains(typescriptType) == false)
                    {
                        ret.Add(typescriptType);
                    }
                }
            }
            return ret;
        }

        public static TypescriptType AddType(Type type, bool fromProperty)
        {
            TypescriptType typescriptType;
            if (store.TryGetValue(type, out typescriptType) == false)
            {
                string currentNamespace = type.Namespace;
                
                typescriptType = new TypescriptType(type, IsExcluded(type));

                typescriptType.NamespaceName = type.Namespace;

                store.Add(type, typescriptType);

                string nonGenericName = GetNonGenericName(type);
                string nonGenericNameWithNamespace = nonGenericName;
                if (string.IsNullOrWhiteSpace(type.Namespace) == false)
                {
                    nonGenericNameWithNamespace = $"{type.Namespace}.{nonGenericName}";
                }
                string baseAffectedName;
                int increment;
                if (IncrementByName.TryGetValue(nonGenericNameWithNamespace, out increment) == true)
                {
                    if (fromProperty)
                    {
                        baseAffectedName = nonGenericName;
                    }
                    else
                    {
                        if (type.IsGenericParameter)
                        {
                            baseAffectedName = nonGenericName;
                        }
                        else
                        {
                            increment++;
                            baseAffectedName = $"{nonGenericName}{increment}";
                            IncrementByName[nonGenericNameWithNamespace] = increment;
                        }
                    }
                }
                else
                {
                    baseAffectedName = nonGenericName;
                    IncrementByName.Add(nonGenericNameWithNamespace, 0);
                }
                List<TypescriptType> typescriptTypeArguments = new List<TypescriptType>();
                Type[] argumentsOfType = null;
                if (type.GenericTypeArguments.Length > 0)
                {
                    argumentsOfType = type.GenericTypeArguments;
                }
                else if (type.GetTypeInfo().GenericTypeParameters.Length > 0)
                {
                    argumentsOfType = type.GetTypeInfo().GenericTypeParameters;
                }
                if (argumentsOfType != null)
                {
                    foreach (Type t in argumentsOfType)
                    {
                        typescriptTypeArguments.Add(AddType(t, fromProperty));
                    }

                    baseAffectedName += "<" + string.Join(",", typescriptTypeArguments.Select(x => x.QualifiedName(type.Namespace))) + ">";
                }


                if (type.IsArray)
                {
                    var typeOfArray = AddType(type.GetElementType(), fromProperty);
                    baseAffectedName = typeOfArray.QualifiedName(currentNamespace) + "[]";
                    typescriptType.AlwaysExcludeNamespace = true;
                }

                if (type.IsGenericType && typeof(Nullable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
                {
                    var typeOfArgument = AddType(type.GetGenericArguments()[0], fromProperty);
                    baseAffectedName = typeOfArgument.QualifiedName(currentNamespace);
                    typescriptType.AlwaysExcludeNamespace = true;
                }

                if (type.IsGenericType && (typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()) || typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
                {
                    var typeOfArgument = AddType(type.GetGenericArguments()[0], fromProperty);
                    baseAffectedName = typeOfArgument.QualifiedName(currentNamespace) + "[]";
                    typescriptType.AlwaysExcludeNamespace = true;
                }

                if (type.IsGenericType && (
                    typeof(Dictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition())
                    || typeof(KeyValuePair<,>).IsAssignableFrom(type.GetGenericTypeDefinition())
                    || typeof(IDictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition())
                    )
                    )
                {
                    var genericArguments = type.GetGenericArguments();

                    if (genericArguments[0].Name == "String" || genericArguments[0].Name == "Int32")
                    {
                        var isInt = genericArguments[0].Name == "Int32";
                        var genericDictionaryArgument = AddType(genericArguments[1], fromProperty);
                        baseAffectedName = $"{{ [key: {(isInt ? "number" : "string")}]: {genericDictionaryArgument.QualifiedName(currentNamespace)} }}";
                    }
                    else
                    {
                        baseAffectedName = $"any/* because unsupported dictionary like key (only string or int32): {genericArguments[0].Name} */";
                    }
                    typescriptType.AlwaysExcludeNamespace = true;
                }

                if (type.IsGenericType && type.Name.StartsWith("Tuple`"))
                {
                    var tupleTab = "\t";
                    var tupleArguments = type.GetGenericArguments();
                    var sbTuple = new StringBuilder();
                    sbTuple.Append($"{{");
                    sbTuple.AppendLine($"");
                    for (var i = 0; i < tupleArguments.Length; i++)
                    {
                        var genericTupleArgument = AddType(tupleArguments[i], fromProperty);
                        sbTuple.AppendLine($"{tupleTab}Item{(i + 1)}: {genericTupleArgument.QualifiedName(currentNamespace)}{(i == (tupleArguments.Length - 1) ? "" : ",")}");
                    }
                    sbTuple.Append($"}}");
                    baseAffectedName = sbTuple.ToString();
                    typescriptType.AlwaysExcludeNamespace = true;
                }

                switch (type.Name.ToLowerInvariant())
                {

                    case "datetime":
                    case "guid":
                        baseAffectedName = "string";
                        typescriptType.AlwaysExcludeNamespace = true;
                        break;
                    case "int16":
                    case "int32":
                    case "int64":
                    case "uint16":
                    case "uint32":
                    case "uint64":
                    case "single":
                    case "double":
                    case "decimal":
                        baseAffectedName = "number";
                        typescriptType.AlwaysExcludeNamespace = true;
                        break;
                    case "boolean":
                        baseAffectedName = "boolean";
                        typescriptType.AlwaysExcludeNamespace = true;
                        break;
                    case "object":
                        baseAffectedName = "any";
                        typescriptType.AlwaysExcludeNamespace = true;
                        break;
                    case "void":
                    case "string":
                        baseAffectedName = type.Name.ToLowerInvariant();
                        typescriptType.AlwaysExcludeNamespace = true;
                        break;
                }





                typescriptType.Type = baseAffectedName;
            }

            if (fromProperty)
            {
                typescriptType.TouchedInProperty = true;
            }
            return typescriptType;
        }

        private static string GetPropertyName(PropertyInfo property, Type type, out string realName)
        {
            var propertyName = property.Name;
            realName = null;
            var customAttributes = property.GetCustomAttributes(true);
            foreach (var customAttribute in customAttributes)
            {
                JsonPropertyAttribute jpa = customAttribute as JsonPropertyAttribute;
                if (jpa != null)
                {
                    propertyName = jpa.PropertyName;
                    realName = propertyName;
                }
            }
            return propertyName;
        }

        private static string FirstCharLowered(string s)
        {
            return Regex.Replace(s, "^.", x => x.Value.ToLowerInvariant());
        }

        private static string GetNonGenericName(Type type)
        {
            string nonGenericName = type.Name;
            // nested classes are put aside parent classes.
            if (nonGenericName == null)
            {
                return null;
            }
            if (nonGenericName.Contains("+"))
            {
                while (nonGenericName.Contains("+"))
                {
                    var lastPositionOfPlus = nonGenericName.LastIndexOf("+");
                    var lastPositionOfDot = nonGenericName.LastIndexOf(".");

                    nonGenericName = nonGenericName.Substring(0, lastPositionOfDot) + "." + nonGenericName.Substring(lastPositionOfPlus + 1);
                }

            }

            int index = nonGenericName.IndexOf('`');
            nonGenericName = index == -1 ? nonGenericName : nonGenericName.Substring(0, index);

            return nonGenericName;
        }

        public static void LoadExclusionRules(List<string> excludedNamespaces, List<string> excludedTypes)
        {
            TypescriptTypeStore.excludedNamespaces = excludedNamespaces;
            TypescriptTypeStore.excludedTypes = excludedTypes;
        }

        private static bool IsExcluded(Type type)
        {
            string typeName = type.FullName;

            var excludedType = excludedTypes.Find(s => type.FullName == s || (s.Contains("*") && IsLikeWildcard(type.FullName, s)));
            if (excludedType == null)
            {
                var excludedNamespace = excludedNamespaces.Find(s => type.FullName != null && type.FullName.StartsWith(s));
                if (excludedNamespace == null)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsLikeWildcard(string type, string wildcard)
        {
            bool ret = false;
            if (type != null)
            {
                var wildcardSet = wildcard.Split(new String[] { "*" }, StringSplitOptions.RemoveEmptyEntries);
                var leftOfType = type;
                for (var i = 0; i < wildcardSet.Length; i++)
                {
                    var wildcardPart = wildcardSet[i];
                    int indexOfWildcard = leftOfType.IndexOf(wildcardPart);
                    if (indexOfWildcard >= 0)
                    {
                        ret = true;
                        leftOfType = leftOfType.Substring(indexOfWildcard + wildcardPart.Length);
                    }
                    else
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }
    }
}
