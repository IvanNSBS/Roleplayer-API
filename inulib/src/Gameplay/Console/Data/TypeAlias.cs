using System;
using System.Collections.Generic;

namespace INUlib.Gameplay.Debugging.Console.Data
{
    public static class TypeAlias
    {
        #region Fields
        private static Dictionary<Type, string> s_typeAlias = new Dictionary<Type, string>
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(object), "object" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(string), "string" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(void), "void" }
        };
        #endregion Fields

        public static string GetPrimitiveTypeAlias(Type parameterType) 
        {
            if (!s_typeAlias.ContainsKey(parameterType)) return "";
            
            return s_typeAlias[parameterType];
        }
    }
}