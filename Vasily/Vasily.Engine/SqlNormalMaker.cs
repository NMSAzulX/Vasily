using System;
using System.Collections.Concurrent;
using Vasily.Core;
using Vasily.Engine.Standard;
using Vasily.Engine.Utils;
using Vasily.Model;

namespace Vasily.Engine
{
    public class SqlNormalMaker : AbstactMaker<SqlModel>
    {
        internal static ConcurrentDictionary<Type, SqlModel> _model_cache;

        public static SqlModel Create(Type type, string splite = null)
        {
            if (!_model_cache.ContainsKey(type))
            {
                new SqlNormalMaker(type, splite);
            }
            return _model_cache[type];
        }
        public static SqlModel Create<T>(string splite = null)
        {
            return Create(typeof(T), splite);
        }


        static SqlNormalMaker()
        {
            _model_cache = new ConcurrentDictionary<Type, SqlModel>();
        }

        public SqlNormalMaker(Type type, string splite=null)
        {
            if (type != null)
            {
                _handler = new AttrOperator(type);

                SqlModel model = new SqlModel(type);

                //分隔符解析
                SetSplite(model, splite);
                //表名解析
                SetTable(model);
                //主键解析
                SetPrimary(model);
                //忽略成员解析
                SetIgnores(model);
                //列名映射解析
                SetColumn(model);
                //生成静态泛型缓存
                StaticGenericModelCache(model,type);
                //生成静态类查询语句
                StaticSqlStringCache(model, type);
                //增加到缓存
                Cache(type, model);
            }
        }

        public override void StaticGenericModelCache(SqlModel model,Type type)
        {
            GsOperator gs = new GsOperator(typeof(SqlModel<>), type);
            gs.Set("PrimaryKey", model.PrimaryKey);
            gs.Set("TableName", model.TableName);
            gs.Set("Left", model.Left);
            gs.Set("Right", model.Right);
            gs.Set("Members", model.Members);
            gs.Set("ColumnMapping", model.ColumnMapping);
            gs.Set("OperatorType", model.OperatorType);
        }

        public override void StaticSqlStringCache(SqlModel model, Type type)
        {
            GsOperator gs = new GsOperator(typeof(SqlEntity<>), type);
            gs["SetPrimary"] = MebOperator.Setter(type, model.PrimaryKey);
            gs["Table"] = model.TableName;
            gs["Primary"] = model.PrimaryKey;

            CountTemplate count = new CountTemplate();
            gs["SelectCount"] = count.SelectCount(model);
            gs["SelectCountWhere"] = count.SelectCountWhere(model);


            SelectTemplate select = new SelectTemplate();
            gs["SelectAll"] = select.SelectAll(model);
            gs["SelectAllWhere"] = select.SelectAllWhere(model);
            gs["SelectAllByPrimary"] = select.SelectAllByPrimary(model);
            gs["SelectAllIn"] = select.SelectAllIn(model);


            UpdateTemplate update = new UpdateTemplate();

            gs["UpdateAllWhere"] = update.UpdateWhere(model);
            gs["UpdateAllByPrimary"] = update.UpdateByPrimary(model);


            InsertTemplate insert = new InsertTemplate();
            gs["InsertAll"] = insert.Insert(model);


            DeleteTemplate delete = new DeleteTemplate();
            gs["DeleteWhere"] = delete.DeleteWhere(model);
            gs["DeleteByPrimary"] = delete.DeleteByPrimary(model);

            RepeateTemplate repeate = new RepeateTemplate();
            var repeateModel = model.ModelWithAttr<NoRepeateAttribute>();
            gs["RepeateCount"] = repeate.RepeateCount(repeateModel);
            gs["RepeateId"] = repeate.RepeateId(repeateModel);
            gs["RepeateEntities"] = repeate.RepeateEntities(repeateModel);
        }


        /// <summary>
        /// 将生成好的Model缓存起来
        /// </summary>
        /// <param name="type">缓存的Key</param>
        /// <param name="maker">model</param>
        public static void Cache(Type type, SqlModel model)
        {
            _model_cache[type] = model;
        }
    }

    public class SqlMaker<T>:SqlNormalMaker {
        public SqlMaker() : base(typeof(T)) { }
    }

}
