using System;

namespace Vasily.Extensions.ReflectionExtensions
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
    }
}
