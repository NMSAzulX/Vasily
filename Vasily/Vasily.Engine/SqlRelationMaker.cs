using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Vasily.Engine.Standard;
using Vasily.Engine.Utils;
using Vasily.Model;

namespace Vasily.Engine
{
    public class SqlRelationMaker : AbstactMaker<SqlRelationModel>
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
            if (type!=null)
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
                _handler = new AttrOperator(types[1]);
                //初始化关系类型
                Init(model);
                //主键解析
                SetPrimary(model);
                //分隔符解析
                SetSplite(model, splite);
                //列名映射解析
                SetColumn(model);
                //表名解析
                SetTable(model);
                //设置主体类Model -- 关联的实体
                model.SetJoinSourceModel(SqlNormalMaker.Create(types[0], splite));

                StaticGenericModelCache(model, type);
                StaticSqlStringCache(model, type);

                //增加到缓存
                Cache(type, model);
            }
           
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="handler"></param>
        public void Init(SqlRelationModel model)
        {
            var Types = model.Types;

            Dictionary <Type, (RelationAttribute Instance, MemberInfo Member)> mappings = new Dictionary<Type, (RelationAttribute Instance, MemberInfo Member)>();

            model.Sources = new string[Types.Length];
            model.AttrMapping = new (RelationAttribute Instance, MemberInfo Member)[Types.Length];
            model.Tables = new string[Types.Length];
            model.AfterTables = new string[Types.Length - 1];
            model.AfterSources = new string[Types.Length - 1];



            //缓存关系注解映射
            ConcurrentDictionary<string, string> _source_mapping = new ConcurrentDictionary<string, string>();
            var relations = _handler.AttrsAndMembers<RelationAttribute>();
            foreach (var item in relations)
            {
                mappings[item.Instance.RelationType] = item;
                _source_mapping[item.Member.Name] = item.Instance.ColumnName;
            }

            //按照外联类型顺序创建对应的成员数组
            for (int i = 0; i < Types.Length; i += 1)
            {
                if (mappings.ContainsKey(Types[i]))
                {
                    model.Sources[i] = mappings[Types[i]].Instance.ColumnName;
                    model.AttrMapping[i] = mappings[Types[i]];
                }
                else
                {
                    model.Sources[i] = mappings[Types[i]].Member.Name;
                }
                model.Tables[i] = mappings[Types[i]].Member.Name;
                if (i > 0)
                {
                    model.AfterTables[i - 1] = model.Tables[i];
                    model.AfterSources[i - 1] = model.Sources[i];
                }
            }

            model.PreTable = model.Tables[0];
            model.PreSource = model.Sources[0];
            model.TableSourceMapping = _source_mapping;
        }

        public override void StaticGenericModelCache(SqlRelationModel model,Type type)
        {
            GsOperator gs = new GsOperator(type);
            gs["Table"] = model.TableName;
            gs["Primary"] = model.PrimaryKey;
            gs["SourceConditions"] = model.Sources;
            gs["TableConditions"] = model.Tables;

        }

        public override void StaticSqlStringCache(SqlRelationModel model, Type type)
        {
            GsOperator gs = new GsOperator(type);
            MemberGetter[] getters = new MemberGetter[model.Sources.Length];
            for (int j = 0; j < model.Sources.Length; j += 1)
            {
                var instance = model.AttrMapping[j].Instance;
                getters[j] = MebOperator.Getter(instance.RelationType, instance.ColumnName);
            }
            gs["Getters"] = getters;


            RelationSelectTemplate select = new RelationSelectTemplate();
            //public static string CountFromTable
            gs["CountFromTable"] = select.SelectCountTable(model);
            //public static string GetFromTable;
            gs["GetFromTable"] = select.SelectEntitesTable(model);

            //public static string CountFromSource
            gs["CountFromSource"] = select.SelectCountSource(model);
            //public static string GetFromSource;
            gs["GetFromSource"] = select.SelectEntitesSource(model);

            RelationUpdateTemplate update = new RelationUpdateTemplate();
            //public static string ModifyFromTable;
            gs["ModifyFromTable"] = update.UpdateTable(model);
            gs["ModifyFromSource"] = update.UpdateSource(model);

            RelationInsertTemplate insert = new RelationInsertTemplate();
            //public static string AddFromTable;
            gs["AddFromTable"] = insert.InsertTable(model);
            gs["AddFromSource"] = insert.InsertSource(model);



            RelationDeleteTemplate delete = new RelationDeleteTemplate();
            //public static string DeleteFromTable;
            gs["DeletePreFromTable"] = delete.DeletePreTable(model);
            gs["DeleteAftFromTable"] = delete.DeleteAftTable(model);

            //public static string DeleteFromSource;
            gs["DeletePreFromSource"] = delete.DeletePreSource(model);
            gs["DeleteAftFromSource"] = delete.DeleteAftSource(model);
        }
    }

    public class SqlRelationMaker<T> : SqlRelationMaker
    {
        public SqlRelationMaker() : base(typeof(T)) { }
    }
}
