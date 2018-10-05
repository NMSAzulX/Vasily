using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;


namespace Vasily.Core
{
    public delegate void StaticSetter(object value);

    public class GsOperator<G, C> : GsOperator {
        public GsOperator() : base(typeof(G).GetGenericTypeDefinition(), typeof(C))
        {

        }
    }

    public class GsOperator<G>: GsOperator
    {
        public GsOperator(Type c_type):base(typeof(G).GetGenericTypeDefinition(), c_type)
        {

        }
    }
    /// <summary>
    /// 泛型类型生成器
    /// </summary>
    public class GsOperator
    {

        private Type _type;
        private Dictionary<string,FieldInfo> _infos;
        private Dictionary<string, StaticSetter> _dynamic_functions;


        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="g_type">泛型外部类型,例如：List<></param>
        /// <param name="c_type">泛型内部类型</param>
        public GsOperator(Type g_type,params Type[] c_type)
        {
            _infos = new Dictionary<string, FieldInfo>();
            _dynamic_functions = new Dictionary<string, StaticSetter>();

            Type _type = g_type.MakeGenericType(c_type);
            FieldInfo[] infos = _type.GetFields(BindingFlags.Static | BindingFlags.Public);

            for (int i = 0; i < infos.Length; i+=1)
            {
                _infos[infos[i].Name] = infos[i];
                _dynamic_functions[infos[i].Name] = CreateSetter(infos[i]);
            }
           
        }

        /// <summary>
        /// 生成Setter方法委托，就是静态字段的emit赋值操作
        /// </summary>
        /// <param name="info">字段</param>
        /// <returns>委托方法</returns>
        internal StaticSetter CreateSetter(FieldInfo info)
        {
            Type type = info.FieldType;
            DynamicMethod method = new DynamicMethod("StaticSetter" + Guid.NewGuid().ToString(), null, new Type[] { typeof(object) });
            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            if (type.IsClass && type != typeof(string) && type != typeof(object))
            {
                il.Emit(OpCodes.Castclass, type);
            }
            else if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            il.Emit(OpCodes.Stsfld, info);
            il.Emit(OpCodes.Ret);
            StaticSetter function = (StaticSetter)(method.CreateDelegate(typeof(StaticSetter)));
            return function;
        }

        /// <summary>
        /// 为静态了字段赋值提供索引操作
        /// </summary>
        /// <param name="field_name">字段名称</param>
        /// <returns></returns>
        public object this[string field_name] {
            set
            {
                if (_dynamic_functions.ContainsKey(field_name))
                {
                    _dynamic_functions[field_name](value);
                }
                else
                {
                    throw new ArgumentNullException("没有这个字段！");
                }
            }
        }
        
        
        /// <summary>
        /// 为当前静态类的字段赋值
        /// </summary>
        /// <param name="field_name">字段名称</param>
        /// <param name="value">值</param>
        public void Set(string field_name, object value)
        {
            if (_dynamic_functions.ContainsKey(field_name))
            {
                _dynamic_functions[field_name](value);
            }
            else
            {
                throw new ArgumentNullException("没有这个字段！");
            }
        }
    }
}
