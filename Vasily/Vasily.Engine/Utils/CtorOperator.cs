using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace Vasily.Engine.Utils
{
    static class CtorOperator
    {
        /// <summary>
        /// 泛型类型Emit初始化动态函数
        /// </summary>
        /// <param name="type">传入的数据库实例类型</param>
        /// <param name="connection">传入的连接字符串</param>
        /// <returns></returns>
        internal static DbCreator DynamicCreateor(Type type,string connection)
        {
            DynamicMethod method = new DynamicMethod("Db" + Guid.NewGuid().ToString(), type, new Type[0]);
            ILGenerator il = method.GetILGenerator();
#if NETSTANDARD1_3
                        ConstructorInfo ctor = info.DeclaringType.GetConstructor(null);
#else
            ConstructorInfo ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(string) }, null);
#endif
            il.Emit(OpCodes.Ldstr, connection);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ret);
            DbCreator function = (DbCreator)(method.CreateDelegate(typeof(DbCreator)));
            return function;
        }

        internal static DbCreator DynamicCreateor<T>(string connection) {
            return DynamicCreateor(typeof(T), connection);
        }
    }
}
