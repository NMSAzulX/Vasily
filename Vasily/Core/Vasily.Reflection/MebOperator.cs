using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Vasily.Core
{
    public delegate object MmberGetter(object value);
    public class MebOperator
    {
        //public void Getter<T>(MemberInfo info)
        //{
        //    DynamicMethod method = new DynamicMethod("MemberGetter" + Guid.NewGuid().ToString(), typeof(object), new Type[] { typeof(object) });
        //    ILGenerator il = method.GetILGenerator();
        //    il.Emit(OpCodes.Ldarg_0);
        //    if (info.MemberType == MemberTypes.Field)
        //    {
        //        il.Emit(OpCodes.Ldfld, ((FieldInfo)info));
        //    }
        //    else if (info.MemberType == MemberTypes.Property)
        //    {
        //        il.Emit(OpCodes.Callvirt, ((PropertyInfo)info));
        //    }
        //    else
        //    {
        //        throw new Exception("无法识别主键反射类型！");
        //    }

        //    il.Emit(OpCodes.Ret);
        //    StaticSetter function = (StaticSetter)(method.CreateDelegate(typeof(StaticSetter)));
        //    return function;
        //}
    }
}
