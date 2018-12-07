using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Vasily.Core
{
    public delegate object MemberGetter(object value);
    public delegate void MemberSetter(object instance, object value);
    public class MebOperator
    {
        public static MemberSetter Setter(Type type, string name)
        {

            DynamicMethod method = new DynamicMethod(name + "MemberSetter", null, new Type[] { typeof(object), typeof(object) });
            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, type);
            il.Emit(OpCodes.Ldarg_1);
           
            PropertyInfo property_info = type.GetProperty(name);

            if (property_info != null)
            {
                if (property_info.PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, property_info.PropertyType);
                }
                MethodInfo method_info = property_info.GetSetMethod(true);
                if (method_info.DeclaringType.IsValueType)
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
                    if (field_info.FieldType.IsValueType)
                    {
                        il.Emit(OpCodes.Unbox_Any, field_info.FieldType);
                    }
                    il.Emit(OpCodes.Stfld, field_info);
                }
            }

            il.Emit(OpCodes.Ret);
            MemberSetter function = (MemberSetter)(method.CreateDelegate(typeof(MemberSetter)));
            return function;
        }
        public static MemberGetter Getter(Type type, string name)
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
                if (method_info.DeclaringType.IsValueType)
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
                var members = type.GetMembers();
                if (members.Length>5)
                {
                    throw new Exception($"在{type}中没有发现{name}字段，请检查代码！");
                }
                else
                {
                    string temp_name = type.Name.Split('_')[0];
                    if (VasilyRunner.RelationExtentsionTyps.ContainsKey(temp_name))
                    {
                        return Getter(VasilyRunner.RelationExtentsionTyps[temp_name],name);
                    }
                    
                }
                
            }
            else
            {
                if (member_type.IsValueType)
                {
                    il.Emit(OpCodes.Box, member_type);
                }
            }
            il.Emit(OpCodes.Ret);
            MemberGetter function = (MemberGetter)(method.CreateDelegate(typeof(MemberGetter)));
            return function;
        }
    }
}
