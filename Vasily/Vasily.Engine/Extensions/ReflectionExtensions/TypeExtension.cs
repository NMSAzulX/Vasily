using System;

namespace Vasily.Engine.Extensions
{
    public static class TypeExtension
    {
        public static Type GetGenericType(this Type type, int index=0)
        {
            if (!type.IsGenericType)
            {
                return type;
            }
            return type.GetGenericArguments()[index];
        }
        public static bool IsNullable(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return true;
            }
            return false;
        }
        public static Type GetNullableType(this Type type)
        {
            return type.GetGenericType(0);
        }
    }
}
