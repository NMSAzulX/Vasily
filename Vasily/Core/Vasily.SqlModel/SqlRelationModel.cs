using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Vasily.Utils;

namespace Vasily.Core
{
    public class SqlRelationModel
    {
        public char Left;
        public char Right;

        public string[] Sources;
        public string[] Tables;

        public string PreTable;
        public string PreSource;

        public string AfterTable;
        public string AfterSource;

        public string[] AfterTables;
        public string[] AfterSources;

        public Type[] Types;
        public SqlModel EntityModel;
        public SqlModel RelationModel;

        public string TableName;
        public SqlType OperatorType;

        public Func<string, string> ColFunction;
        public Func<string, string> SourceFunction;
        public Func<string, string> FilterFunction;

        public ConcurrentDictionary<string, string> ColumnMapping;
        public ConcurrentDictionary<string, string> TableSourceMapping;
        public (RelationAttribute Instance, MemberInfo Member)[] AttrMapping;


        public Type EntityType;
        private AttrOperator _handler;



        public SqlRelationModel(Type type,Type[] types)
        {
            EntityType = type;
            _handler = new AttrOperator(type);
            Types = types;
            EntityModel = SqlMaker.Create(types[0]);
            ColFunction = (item) => { return RealColumn(item); };
            SourceFunction = (item) => { return SourceColumn(item); };
            
        }

        public void UseDefaultFilter()
        {
            FilterFunction = (item) =>
            {
                return SourceColumn(item);
            };
        }
        public void ClearFilter()
        {
            FilterFunction = null;
        }

        public string SourceColumn(string item)
        {
            if (TableSourceMapping.ContainsKey(item))
            {
                return TableSourceMapping[item];
            }
            return item;
        }

        /// <summary>
        /// 设置主体类Model
        /// </summary>
        /// <param name="model"></param>
        public void SetModel(SqlModel model)
        {
            EntityModel = model;
        }
        /// <summary>
        /// 解析分隔符
        /// </summary>
        /// <param name="splite"></param>
        public void SetSplite(string splite)
        {
            
            if (splite == null)
            {
                Left = default(char);
                Right = default(char);
            }
            else
            {
                Left = splite[0];
                Right = splite[1];
            }
        }

        /// <summary>
        /// 解析自身Model
        /// </summary>
        /// <param name="type"></param>
        /// <param name="splite"></param>
        public void SetSelfModel( string splite=null)
        {
            RelationModel = SqlMaker.Create(EntityType,splite);
        }

        /// <summary>
        /// 映射函数
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string RealColumn(string item)
        {
            if (ColumnMapping.ContainsKey(item))
            {
                return ColumnMapping[item];
            }
            return item;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="handler"></param>
        public void Init(AttrOperator handler=null)
        {
            if (handler == null)
            {
                handler = _handler;
            }
            Dictionary<Type,(RelationAttribute Instance, MemberInfo Member)> mappings = new Dictionary<Type, (RelationAttribute Instance, MemberInfo Member)>();

            Sources = new string[Types.Length];
            AttrMapping = new (RelationAttribute Instance, MemberInfo Member)[Types.Length];
            Tables = new string[Types.Length];
            AfterTables = new string[Types.Length-1];
            AfterSources = new string[Types.Length - 1];

            


            //缓存关系注解映射
            ConcurrentDictionary<string, string> _source_mapping = new ConcurrentDictionary<string, string>();
            var relations = handler.AttrsAndMembers<RelationAttribute>();
            foreach (var item in relations)
            {
                mappings[item.Instance.RelationType] = item;
                _source_mapping[item.Member.Name] = item.Instance.ColumnName;
            }
            
            //按照外联类型顺序创建对应的成员数组
            for (int i = 0; i < Types.Length; i+=1)
            {
                if (mappings.ContainsKey(Types[i]))
                {
                    Sources[i] = mappings[Types[i]].Instance.ColumnName;
                    AttrMapping[i] = mappings[Types[i]];
                }
                else
                {
                    Sources[i] = mappings[Types[i]].Member.Name;
                }
                Tables[i] = mappings[Types[i]].Member.Name;
                if (i>0)
                {
                    AfterTables[i-1] = Tables[i];
                    AfterSources[i - 1] = Sources[i];
                }
            }

            PreTable = Tables[0];
            PreSource = Sources[0];
            TableSourceMapping = _source_mapping;
        }


        public void SetColumn(AttrOperator handler=null)
        {
            if (handler == null)
            {
                handler = _handler;
            }
            ConcurrentDictionary<string, string> _column_mapping = new ConcurrentDictionary<string, string>();
           
            var mappings = handler.AttrsAndMembers<ColumnAttribute>();
            foreach (var item in handler._members)
            {
                _column_mapping[item.Name] = item.Name;
               
            }
            foreach (var item in mappings)
            {
                _column_mapping[item.Member.Name] = item.Instance.Name;
               
            }
            ColumnMapping = _column_mapping;
        }
        public void SetTable(AttrOperator handler=null)
        {
            if (handler == null)
            {
                handler = _handler;
            }
            var table = handler.Instance<TableAttribute>();
            if (table == null)
            {
                throw new NullReferenceException($"{handler._type}类不存在Table注解，请检查实体类！");
            }
            TableName = table.Name;
            OperatorType = table.Type;
            if (Left==default(char))
            {
                SetSplite(SqlSpliter.GetSpliter(OperatorType));
            }
        }
    }

}
