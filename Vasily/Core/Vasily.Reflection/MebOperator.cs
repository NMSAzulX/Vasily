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
        public static PropertyGetter Getter(Type type, string name)
        {
            DynamicMethod method = new DynamicMethod(name + "MemberGetter", typeof(object), new Type[] { typeof(object) });
            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, type);

            Type member_type = null;
            PropertyInfo property_info = type.GetProperty(name);

            if (property_info != null)
            {
                member_type = property_info.PropertyType;
                MethodInfo method_info = property_info.GetGetMethod(true);
                if (method_info.DeclaringType.IsValueType || method_info.IsStatic)
                {
                    il.Emit(OpCodes.Call, method_info);
                }
                else
                {
                    il.Emit(OpCodes.Callvirt, method_info);
                }
            }
            else
            {
                FieldInfo field_info = type.GetField(name);
                if (field_info != null)
                {
                    member_type = field_info.FieldType;
                    il.Emit(OpCodes.Ldfld, field_info);
                }
            }

            if (member_type == null)
            {
                throw new Exception($"在{type}中没有发现{name}字段，请检查代码！");
            }
            else
            {
                if (member_type.IsValueType)
                {
                    il.Emit(OpCodes.Box, member_type);
                }
            }
            il.Emit(OpCodes.Ret);
            PropertyGetter function = (PropertyGetter)(method.CreateDelegate(typeof(PropertyGetter)));
            return function;
        }
    }
}
