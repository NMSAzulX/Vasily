using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Vasily.Core
{
    public class AttrOperator
    {
        private static ConcurrentDictionary<Type, MemberInfo[]> _member_cache;
        internal Type _type;
        internal MemberInfo[] _members;

        static AttrOperator()
        {
            _member_cache = new ConcurrentDictionary<Type, MemberInfo[]>();
        }
        public AttrOperator(Type type)
        {
            if (type==null)
            {
                return;
            }

            List<MemberInfo> cache = new List<MemberInfo>();
            _type = type;
            if (!_member_cache.ContainsKey(type))
            {
                _type.GetFields();
                cache.AddRange(_type.GetFields());
                cache.AddRange(_type.GetProperties());
                _member_cache[type] = cache.ToArray();
                cache.Clear();
            }
            _members = _member_cache[type];
        }


        /// <summary>
        /// 根据类型返回匹配标签的成员集合
        /// </summary>
        /// <param name="type">标签类型</param>
        /// <returns>符合条件的成员集合</returns>
        public IEnumerable<MemberInfo> Members(Type attributeType)
        {
            List<MemberInfo> memberInfos = new List<MemberInfo>();
            for (int i = 0; i < _members.Length; i += 1)
            {
                Attribute result = _members[i].GetCustomAttribute(attributeType);
                if (result!=null)
                {
                    memberInfos.Add(_members[i]);
                }
            }
            return memberInfos;
        }
        public IEnumerable<MemberInfo> Members<T>()
        {
            return Members(typeof(T));
        }


        /// <summary>
        /// 根据类型返回匹配标签的第一个匹配的成员
        /// </summary>
        /// <param name="type">标签类型</param>
        /// <returns>符合条件的成员</returns>
        public MemberInfo Member(Type attributeType)
        {
            for (int i = 0; i < _members.Length; i += 1)
            {
                Attribute result = _members[i].GetCustomAttribute(attributeType);
                if (result != null)
                {
                    return _members[i];
                }
            }
            return null;
        }
        public MemberInfo Member<T>()
        {
            return Member(typeof(T));
        }
        /// <summary>
        /// 根据标签类型返回标签实例集合.(标签在属性上唯一)
        /// </summary>
        /// <typeparam name="T">标签类型</typeparam>
        /// <returns>实例与成员字键值对结果集</returns>
        public List<(T Instance, MemberInfo Member)> AttrsAndMembers<T>() where T : Attribute
        {
            List<(T Instance, MemberInfo Member)> memberInfos = new List<(T Instance, MemberInfo Member)>();
            Type findType = typeof(T);
            for (int i = 0; i < _members.Length; i += 1)
            {
                T result = _members[i].GetCustomAttribute<T>();
                if (result != null)
                {
                    memberInfos.Add((result, _members[i]));
                }
            }
            return memberInfos;
        }


        /// <summary>
        /// 根据标签类型返回标签实例集合.(标签在属性上唯一)
        /// </summary>
        /// <typeparam name="T">标签类型</typeparam>
        /// <returns>实例与成员键值对</returns>
        public (T Instance, MemberInfo Member) AttrAndMember<T>() where T : Attribute
        {
            for (int i = 0; i < _members.Length; i += 1)
            {
                T result = _members[i].GetCustomAttribute<T>();

                if (result != null)
                {
                    return (result, _members[i]);
                }

            }
            return (null, null);
        }

        /// <summary>
        /// 返回该类型上的标签实例
        /// </summary>
        /// <typeparam name="T">标签类型</typeparam>
        /// <returns>标签实例</returns>
        public T Instance<T>() where T : Attribute
        {
            T instance = _type.GetCustomAttribute<T>();
            return instance;
        }

        /// <summary>
        /// 返回该类型上的标签实例
        /// </summary>
        /// <typeparam name="T">标签类型</typeparam>
        /// <returns>标签实例</returns>
        public IEnumerable<T> Instances<T>() where T : Attribute
        {
            IEnumerable<T> instances = _type.GetCustomAttributes<T>();
            return instances;
        }
    }
}
