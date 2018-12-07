using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Vasily.Utils;

namespace Vasily.Core
{
    public class SqlRelationMaker
    {
        internal static ConcurrentDictionary<Type, SqlRelationModel> _model_cache;

        public static SqlRelationModel Create(Type type, string splite = null)
        {
            if (!_model_cache.ContainsKey(type))
            {
                new SqlRelationMaker(type, splite);
            }
            return _model_cache[type];
        }
        public static SqlRelationModel Create<T>(string splite = null)
        {
            return Create(typeof(T), splite);
        }
        static SqlRelationMaker()
        {
            _model_cache = new ConcurrentDictionary<Type, SqlRelationModel>();
        }

        public static void Cache(Type type, SqlRelationModel maker)
        {
            _model_cache[type] = maker;
        }

        public SqlRelationMaker(Type osRelation, string splite = null) : this(osRelation, osRelation.GetGenericArguments(), splite)
        {

        }
        public SqlRelationMaker(Type type, Type[] types, string splite = null)
        {

            Type[] temp_types = new Type[types.Length - 1];


            int index = 0;
            for (int i = 0; i < temp_types.Length; i += 1)
            {
                if (i == 1)
                {
                    index += 1;
                }
                temp_types[i] = types[index];
                index += 1;
            }

            if (type == null)
            {
                return;
            }
            //创建标签Helper

            SqlRelationModel model = new SqlRelationModel(types[1], temp_types);

            //初始化关系类型
            model.Init();
            //分隔符解析
            model.SetSplite(splite);
            //添加外层关系
            model.SetModel(SqlMaker.Create(types[0], splite));
            //列名映射解析
            model.SetColumn();
            //表名解析
            model.SetTable();
            //RelationModel
            model.SetSelfModel(splite);
            //增加到缓存
            Cache(type, model);
        }
    }

    public class SqlRelationMaker<T> : SqlRelationMaker
    {
        public SqlRelationMaker() : base(typeof(T)) { }
    }
}
