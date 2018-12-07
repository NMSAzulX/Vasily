using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Vasily.Core
{
    public class SqlMaker
    {
        internal static ConcurrentDictionary<Type, SqlModel> _model_cache;

        public static SqlModel Create(Type type, string splite = null)
        {
            if (!_model_cache.ContainsKey(type))
            {
                new SqlMaker(type, splite);
            }
            return _model_cache[type];
        }
        public static SqlModel Create<T>(string splite = null)
        {
            return Create(typeof(T), splite);
        }
        static SqlMaker()
        {
            _model_cache = new ConcurrentDictionary<Type, SqlModel>();
        }

        public static void Cache(Type type,SqlModel maker)
        {
            _model_cache[type] = maker;
        }

        public SqlMaker(Type type, string splite=null)
        {
            if (type == null)
            {
                return;
            }

            SqlModel model = new SqlModel(type);

            //分隔符解析
            model.SetSplite(splite);
            //表名解析
            model.SetTable();
            //主键解析
            model.SetPrimary();
            //忽略成员解析
            model.SetIgnores();
            //列名映射解析
            model.SetColumn();
            //生成静态泛型缓存
            model.CacheGeneric(type);

            //增加到缓存
            Cache(type, model);
        }

    }

    public class SqlMaker<T>:SqlMaker {
        public SqlMaker() : base(typeof(T)) { }
    }

}
