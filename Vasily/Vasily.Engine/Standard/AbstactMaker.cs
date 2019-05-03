using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Vasily.Engine.Extensions;
using Vasily.Engine.Utils;
using Vasily.Model;
namespace Vasily.Engine.Standard
{
    public class AbstactMaker<T> where T: SqlModel
    {
        internal AttrOperator _handler;
        /// <summary>
        /// 手动设置分隔符
        /// </summary>
        /// <param name="splite"></param>
        public static void SetSplite(T model, string splite)
        {
            if (splite == null)
            {
                model.Left = default(char);
                model.Right = default(char);
            }
            else
            {
                model.Left = splite[0];
                model.Right = splite[1];
            }
        }

        /// <summary>
        /// 解析列映射列表
        /// </summary>
        /// <param name="handler"></param>
        public void SetColumn(T model)
        {
            ConcurrentDictionary<string, string> _column_mapping = new ConcurrentDictionary<string, string>();
            var mappings = _handler.AttrsAndMembers<ColumnAttribute>();
            foreach (var item in _handler._members)
            {
                _column_mapping[item.Name] = item.Name;
            }
            foreach (var item in mappings)
            {
                _column_mapping[item.Member.Name] = item.Instance.Name;
            }
            model.ColumnMapping = _column_mapping;
            model.ResetMembers(_column_mapping.Values.ToArray());
        }


        /// <summary>
        /// 解析表注解，完善分隔符
        /// </summary>
        /// <param name="handler"></param>
        public void SetTable(T model)
        {
            var table = _handler.Instance<TableAttribute>();
            if (table == null)
            {
                throw new NullReferenceException($"{_handler._type}类不存在Table注解，请检查实体类！");
            }
            model.TableName = table.Name;
            model.OperatorType = table.Type;
            if (model.Left == default(char))
            {
                SetSplite(model,SqlSpliter.GetSpliter(model.OperatorType));
            }
        }

        // <summary>
        /// 解析忽略列表
        /// </summary>
        /// <param name="handler"></param>
        public void SetIgnores(T model)
        {

            var ignores = _handler.Members<IgnoreAttribute>();
            List<string> list = new List<string>();
            foreach (var item in ignores)
            {
                list.Add(item.Name);
            }
            if (model.Members == null)
            {
                SetColumn(model);
            }
            model.RemoveMembers(list);
        }
        public SqlModel ModelWithoutAttr<A>(T model)
        {
            var newModel = model.CopyInstance();
            newModel.FilterFunction = model.FilterFunction;
            var attrMembers = _handler.Members(typeof(T));
            newModel.RemoveMembers(attrMembers.GetNames());
            return newModel;
        }
        public SqlModel ModelWithAttr<A>(T model)
        {
            var newModel = model.CopyInstance();
            newModel.FilterFunction = model.FilterFunction;
            var attrMembers = _handler.Members(typeof(T));
            newModel.ResetMembers(attrMembers.GetNames());
            return newModel;
        }

        /// <summary>
        /// 解析主键
        /// </summary>
        /// <param name="handler"></param>
        public void SetPrimary(T model)
        {
            var primary = _handler.AttrAndMember<PrimaryKeyAttribute>();

            if (primary.Instance != null)
            {
                model.PrimaryKey = primary.Member.Name;
                model.PrimaryManually = primary.Instance.IsManually;
            }
            else
            {
                model.PrimaryKey = null;
            }
        }



        /// <summary>
        /// 构造静态泛型类缓存
        /// </summary>
        /// <param name="type"></param>
        public virtual void StaticGenericModelCache(T model,Type type) { }

        /// <summary>
        /// 生成静态类SQL语句
        /// </summary>
        /// <param name="model"></param>
        /// <param name="type"></param>
        public virtual void StaticSqlStringCache(T model,Type type)
        {

        }
    }
}
