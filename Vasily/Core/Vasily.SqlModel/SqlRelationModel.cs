using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Vasily.Utils;

namespace Vasily.Core
{
    public class SqlRelationModel:SqlModel
    {
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
        //public SqlModel RelationModel;

        public Func<string, string> SourceFunction;

        public ConcurrentDictionary<string, string> TableSourceMapping;
        public (RelationAttribute Instance, MemberInfo Member)[] AttrMapping;



        public SqlRelationModel(Type type,Type[] types)
        {
            EntityType = type;
            _handler = new AttrOperator(type);
            Types = types;
            SourceFunction = (item) => { return SourceColumn(item); };
            
        }

        public void UseDefaultFilter()
        {
            FilterFunction = (item) =>
            {
                return SourceColumn(item);
            };
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
        public void SetJoinSourceModel(SqlModel model)
        {
            EntityModel = model;
        }

    }

}
