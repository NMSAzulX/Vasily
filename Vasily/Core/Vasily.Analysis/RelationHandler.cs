using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Vasily.Standard;

namespace Vasily.Core
{
    public partial class RelationHandler : HandlerBase
    {
        private ISelect _select_template;
        private IUpdate _update_template;
        private IDelete _delete_template;
        private IInsert _insert_template;
        private Dictionary<MemberInfo, RelationAttribute> _mapping;
        public RelationHandler(string splites, Type entity_type) : base(splites, entity_type)
        {
            _mapping = new Dictionary<MemberInfo, RelationAttribute>();
            _select_template = new SelectTemplate();
            _update_template = new UpdateTemplate();
            _delete_template = new DeleteTemplate();
            _insert_template = new InsertTemplate();
            Store();
        }

        public void Store()
        {
            var mappings = _handler.Mappings<RelationAttribute>();
            foreach (var item in mappings)
            {
                _mapping[item.Member] = item.Instance;
            }

            var members = new List<MemberInfo>(_handler.Members<RelationAttribute>());

            //传入到排列树
            PermutationTree<MemberInfo> trees = new PermutationTree<MemberInfo>(members.ToArray());

            //获取排列结果
            List<List<MemberInfo>> results = trees.SumA(2, members.Count);

            //多泛型生成器实例
            GsOperator gs = null;

            //关联关系排列类型
            Type[] ts = null;

            //循环关联关系，每个元素是一种排列
            for (int i = 0; i < results.Count; i += 1)
            {

                int gsCount = results[i].Count;

                //获取此种成员排列对应的类型排列
                ts = GetTypes(results[i].ToArray());
               
                switch (gsCount)
                {
                    case 2:

                        gs = new GsOperator(typeof(RelationSql<,,>), ts);
                        break;

                    case 3:

                        gs = new GsOperator(typeof(RelationSql<,,,>), ts);
                        break;

                    case 4:

                        gs = new GsOperator(typeof(RelationSql<,,,,>), ts);
                        break;

                    case 5:

                        gs = new GsOperator(typeof(RelationSql<,,,,,>), ts);
                        break;

                    case 6:

                        gs = new GsOperator(typeof(RelationSql<,,,,,,>), ts);
                        break;
                    case 7:

                        gs = new GsOperator(typeof(RelationSql<,,,,,,,>), ts);
                        break;

                    default:
                        break;
                }

                Func<MemberInfo, string> filter = (item) =>
                {
                    if (_mapping.ContainsKey(item))
                    {
                        return _mapping[item].ColumnName;
                    }
                    return item.Name;
                };

                MemberInfo[] parameter = results[i].ToArray();

                string[] sources = new string[parameter.Length];
                string[] table = new string[parameter.Length];
                for (int j = 0; j < parameter.Length; j+=1)
                {
                    table[j] = parameter[j].Name;
                    sources[j] = filter(parameter[j]);
                }

                MemberGetter[] getters = new MemberGetter[parameter.Length];
                for (int j = 0; j < sources.Length; j += 1)
                {
                    var instance = _mapping[parameter[j]];
                    getters[j] = MebOperator.Getter(instance.RelationType, instance.ColumnName);
                   
                }

                gs["Table"] = _model.TableName;
                gs["Primary"] = _model.PrimaryKey;

                gs.Set("SourceConditions", sources);
                gs.Set("TableConditions", table);
                gs.Set("Getters", getters);

                //public static string GetFromTable;
                gs.Set("GetFromTable", SelectString(parameter));
                //public static string ModifyFromTable;
                gs.Set("ModifyFromTable", UpdateString(parameter));
                //public static string DeleteFromTable;
                gs.Set("DeletePreFromTable", DeletePreString(parameter[0]));
                gs.Set("DeleteAftFromTable", DeleteAftString(parameter));
                //public static string AddFromTable;
                gs.Set("AddFromTable", InsertString(parameter));


                
                
                //public static string GetFromSource;
                gs.Set("GetFromSource", SelectString(parameter, filter));
                //public static string ModifyFromSource;
                gs.Set("ModifyFromSource", UpdateString(parameter, filter));
                //public static string DeleteFromSource;
                gs.Set("DeletePreFromSource", DeletePreString(parameter[0], filter));
                gs.Set("DeleteAftFromSource", DeleteAftString(parameter, filter));
                //public static string AddFromSource;
                gs.Set("AddFromSource", InsertString(parameter,filter));
            }
        }

        /// <summary>
        /// 根据Member集合获取对应标签中的类型集合
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public Type[] GetTypes(MemberInfo[] values)
        {
            Type[] types = new Type[values.Length + 1];
            int index = 0;
            for (int i = 0; i < values.Length; i += 1)
            {
                if (i == 1)
                {
                    types[index] = _entity_type;
                    index += 1;
                }
                types[index] = _mapping[values[i]].RelationType;
                index += 1;
            }
            return types;
        }

        /// <summary>
        /// 获取SELECT [member1] FROM [TableName] WHERE [member2]=@member2 AND = [member3]=@member3
        /// </summary>
        /// <param name="members">成员集合</param>
        /// <returns>返回条件查询SQL</returns>
        public string SelectString(MemberInfo[] members, Func<MemberInfo, string> filter = null)
        {
            var model = _model.CopyInstance();
            model.FilterFunction = filter;
            MemberInfo[] temp = new MemberInfo[members.Length - 1];
            Array.Copy(members, 1, temp, 0, members.Length - 1);
            model.LoadMembers(members[0]);
            return _select_template.SelectWithCondition(model, temp);
        }

        /// <summary>
        /// 获取更新语句
        /// </summary>
        /// <param name="filter">将参数化映射到标签中类的主键或其他字段上</param>
        /// <returns></returns>
        public string UpdateString(MemberInfo[] members, Func<MemberInfo, string> filter = null)
        {
            var model = _model.CopyInstance();
            model.FilterFunction = filter;
            MemberInfo[] temp = new MemberInfo[members.Length - 1];
            Array.Copy(members, 1, temp, 0, members.Length-1);
            model.LoadMembers(members[0]);
            return _update_template.UpdateWithCondition(model, temp);
        }

        /// <summary>
        /// 获取删除语句
        /// </summary>
        /// <param name="filter">将参数化映射到标签中类的主键或其他字段上</param>
        /// <returns></returns>
        public string DeletePreString(MemberInfo member, Func<MemberInfo, string> filter = null)
        {
            if (filter != null)
            {
                var model = _model.CopyInstance();
                model.FilterFunction = filter;
                return _delete_template.DeleteWithCondition(model, member);
            }
            return _delete_template.DeleteWithCondition(_model, member);
        }
        /// <summary>
        /// 获取删除语句
        /// </summary>
        /// <param name="filter">将参数化映射到标签中类的主键或其他字段上</param>
        /// <returns></returns>
        public string DeleteAftString(MemberInfo[] members, Func<MemberInfo, string> filter = null)
        {

            MemberInfo[] temp = new MemberInfo[members.Length - 1];
            Array.Copy(members, 1, temp, 0, members.Length - 1);

            if (filter != null)
            {
                var model = _model.CopyInstance();
                model.FilterFunction = filter;
                return _delete_template.DeleteWithCondition(model, temp);
            }
            return _delete_template.DeleteWithCondition(_model, temp);
        }

        /// <summary>
        /// 获取插入语句
        /// </summary>
        /// <param name="filter">将参数化映射到标签中类的主键或其他字段上</param>
        /// <returns></returns>
        public string InsertString(MemberInfo[] values, Func<MemberInfo, string> filter = null)
        {
            var model = _model.CopyInstance();
            model.FilterFunction = filter;
            model.LoadMembers(values);
            return _insert_template.Insert(model);

        }
    }
}
