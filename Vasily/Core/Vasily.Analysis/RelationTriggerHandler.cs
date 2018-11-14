using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Vasily.Standard;
using Vasily.Utils;

namespace Vasily.Core
{
    public partial class RelationTriggerHandler:BaseHandler
    {
        internal RelationTemplate Handler;
        private Dictionary<MemberInfo, RelationAttribute> _mapping;
        private Type[] _types;
        private MemberInfo[] _parameters;
        internal Func<MemberInfo, string> _filter;
        public MemberGetter[] Getters;
        public string[] Sources;
        public string[] Tables;

        public RelationTriggerHandler(Type osRelation) : this(osRelation.GetGenericArguments())
        {

            System.Diagnostics.Debug.WriteLine(osRelation.GetGenericArguments());
        }
        public RelationTriggerHandler(Type[] types) : base(SqlSpliter.GetSpliter(types[1]), types[1])
        {

            //获取实际静态泛型参数数组
            _types = types;
            _mapping = new Dictionary<MemberInfo, RelationAttribute>();

            //获取关系标签数组
            var mappings = _handler.Mappings<RelationAttribute>();

            //创建类型到成员的映射缓存
            Dictionary<Type, MemberInfo> memberDict = new Dictionary<Type, MemberInfo>();
            foreach (var item in mappings)
            {
                _mapping[item.Member] = item.Instance;
                memberDict[item.Instance.RelationType] = item.Member;
            }
            //获取<T,R,C1,C2....>中的<T,C1,C2>
            _parameters = new MemberInfo[_types.Length - 1];
            int index = 0;
            for (int i = 0; i < _parameters.Length; i+=1)
            {
                if (i==1)
                {
                    index += 1;
                }
                _parameters[i] = memberDict[_types[index]];
                index += 1;
            }

            //创建表到实体类的映射委托
            _filter = (item) =>
            {
                if (_mapping.ContainsKey(item))
                {
                    string result = _mapping[item].ColumnName;
                    if (result != null)
                    {
                        return result;
                    }
                }
                return item.Name;
            };

            Sources = new string[_parameters.Length];
            Tables = new string[_parameters.Length];

            for (int j = 0; j < _parameters.Length; j += 1)
            {
                Tables[j] = _parameters[j].Name;
                Sources[j] = _filter(_parameters[j]);
            }

            //创建Relation标签中 RelationType.ColumnName的Emit委托
            Getters = new MemberGetter[_parameters.Length];
            for (int j = 0; j < Sources.Length; j += 1)
            {
                var instance = _mapping[_parameters[j]];
                Getters[j] = MebOperator.Getter(instance.RelationType, instance.ColumnName);

            }
            //创建关系模板
            Handler = new RelationTemplate(_model, _parameters);
        }
    }
}
