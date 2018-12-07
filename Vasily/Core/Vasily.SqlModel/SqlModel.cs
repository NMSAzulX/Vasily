using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Vasily.Core;
using Vasily.Extensions;
using Vasily.Utils;

namespace Vasily
{
    public class SqlModel
    {
        public char Left;
        public char Right;
        public string TableName;
        public SqlType OperatorType;
        public string PrimaryKey;
        public bool PrimaryManually;
        public HashSet<string> Members;
        public Func<string, string> ColFunction;
        public Func<string, string> FilterFunction;
        public ConcurrentDictionary<string, string> ColumnMapping;
        public Type EntityType;
        private AttrOperator _handler;
        

        public SqlModel(Type type)
        {
            ColFunction = (item) => { return Column(item); };
            EntityType = type;
            _handler = new AttrOperator(type);
        }


        public SqlModel ModelWithoutAttr<T>(AttrOperator handler=null)
        {
            if (handler==null)
            {
                handler = _handler;
            }
            var model = CopyInstance();
            model.FilterFunction = FilterFunction;
            var attrMembers = handler.Members(typeof(T));
            model.RemoveMembers(attrMembers.GetNames());
            return model;
        }
        public SqlModel ModelWithAttr<T>(AttrOperator handler = null)
        {
            if (handler == null)
            {
                handler = _handler;
            }
            var model = CopyInstance();
            model.FilterFunction = FilterFunction;
            var attrMembers = handler.Members(typeof(T));
            model.ResetMembers(attrMembers.GetNames());
            return model;
        }

        /// <summary>
        /// 生成一个新的，忽略掉主键的Model
        /// </summary>
        /// <returns></returns>
        public SqlModel ModelWithoutPrimary()
        {
            var model = CopyInstance();
            model.RemoveMembers(PrimaryKey);
            model.FilterFunction = FilterFunction;
            return model;
        }

        /// <summary>
        /// 手动设置分隔符
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
        /// 解析表注解，完善分隔符
        /// </summary>
        /// <param name="handler"></param>
        public void SetTable(AttrOperator handler=null)
        {
            if (handler == null)
            {
                handler = _handler;
            }
            var table = handler.ClassInstance<TableAttribute>();
            if (table==null)
            {
                throw new NullReferenceException($"{handler._type}类不存在Table注解，请检查实体类！");
            }
            TableName = table.Name;
            OperatorType = table.Type;
            if (Left == default(char))
            {
                SetSplite(SqlSpliter.GetSpliter(OperatorType));
            }
        }

        /// <summary>
        /// 解析主键
        /// </summary>
        /// <param name="handler"></param>
        public void SetPrimary(AttrOperator handler=null)
        {
            if (handler == null)
            {
                handler = _handler;
            }
            var primary = handler.Mapping<PrimaryKeyAttribute>();

            if (primary.Instance != null)
            {
                PrimaryKey = primary.Member.Name;
                PrimaryManually = primary.Instance.IsManually;
            }
            else
            {
                PrimaryKey = null;
            }
        }

        /// <summary>
        /// 解析忽略列表
        /// </summary>
        /// <param name="handler"></param>
        public void SetIgnores(AttrOperator handler=null)
        {
            if (handler == null)
            {
                handler = _handler;
            }
            var ignores = handler.Members<IgnoreAttribute>();
            List<string> list = new List<string>();
            foreach (var item in ignores)
            {
                list.Add(item.Name);
            }
            if (Members==null)
            {
                SetColumn(handler);
            }
            RemoveMembers(list);
        }
        /// <summary>
        /// 解析列映射列表
        /// </summary>
        /// <param name="handler"></param>
        public void SetColumn(AttrOperator handler=null)
        {
            if (handler == null)
            {
                handler = _handler;
            }
            ConcurrentDictionary<string, string> _column_mapping = new ConcurrentDictionary<string, string>();
            var mappings = handler.Mappings<ColumnAttribute>();
            foreach (var item in handler._members)
            {
                _column_mapping[item.Name] = item.Name;
            }
            foreach (var item in mappings)
            {
                _column_mapping[item.Member.Name] = item.Instance.Name;
            }
            ColumnMapping = _column_mapping;
            ResetMembers(_column_mapping.Values.ToArray());
        }

        /// <summary>
        /// 生成静态缓存
        /// </summary>
        /// <param name="type"></param>
        public void CacheGeneric(Type type)
        {
            //静态sql生成器。例如 MakerModel<Student>
            GsOperator gs = new GsOperator(typeof(SqlModel<>),type);
            gs.Set("PrimaryKey", PrimaryKey);
            gs.Set("TableName", TableName);
            gs.Set("Left", Left);
            gs.Set("Right", Right);
            gs.Set("Members", Members);
            gs.Set("ColumnMapping", ColumnMapping);
            gs.Set("OperatorType", OperatorType);

        }
        /// <summary>
        /// 根据ColumnAttribute的映射返回实际字段名
        /// </summary>
        /// <param name="item">当前属性</param>
        /// <returns>数据库中实际字段名</returns>
        public string Column(string item)
        {
            if (ColumnMapping.ContainsKey(item))
            {
                return ColumnMapping[item];
            }
            return item;
        }

        /// <summary>
        /// 成员集合求并集
        /// </summary>
        /// <param name="members">需要合并的集合</param>
        public void AddMembers(IEnumerable<string> members)
        {
            if (Members == null)
            {
                throw new NullReferenceException("成员数组不能为空，为空不能做添加操作！");
            }
            Members.UnionWith(members);
        }

        /// <summary>
        /// 加载一个新的集合
        /// </summary>
        /// <param name="members">新集合</param>
        public void ResetMembers(params string[] members)
        {
            Members = null;
            Members = new HashSet<string>(members);
        }
        public void ResetMembers(IEnumerable<string> members)
        {
            Members = null;
            Members = new HashSet<string>(members);
        }

        /// <summary>
        /// 排除集合
        /// </summary>
        /// <param name="ignores">需要排除的集合</param>
        public void RemoveMembers(IEnumerable<string> ignores)
        {
            if (Members == null)
            {
                throw new NullReferenceException("成员数组不能为空，为空不能做排除操作！");
            }
            Members.ExceptWith(ignores);
        }
        public void RemoveMembers(params string[] ignores)
        {
            if (Members == null)
            {
                throw new NullReferenceException("成员数组不能为空，为空不能做排除操作！");
            }
            Members.ExceptWith(ignores);
        }
        public SqlModel CopyInstance()
        {
            SqlModel newModel = new SqlModel(EntityType)
            {
                Left = Left,
                Right = Right,
                TableName = TableName,
                PrimaryKey = PrimaryKey,
                PrimaryManually = PrimaryManually,
                ColumnMapping = ColumnMapping,
                OperatorType = OperatorType
            };
            newModel.Members = new HashSet<string>(Members);
            return newModel;
        }
    }

    /// <summary>
    /// 当初始化MakerModel的时候，会产生静态泛型副本，以便后续直接用模板处理
    /// 其中Left,Right,TableName,PrimaryKey,Members会被复制
    /// Left,Right为SQL内置关键字分隔符
    /// TableName为表名
    /// PrimaryKey主键名
    /// Members成员信息
    /// ColFunction 为组建SQL语句时用到的过滤函数
    /// FilterFunction 为组建@参数时用到的过滤函数
    /// ColumnMapping 为Column的映射的缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class SqlModel<T>
    {
        public static char Left;
        public static char Right;
        public static string TableName;
        public static string PrimaryKey;
        public static bool PrimaryManually;
        public static HashSet<string> Members;
        public static Func<string, string> ColFunction;
        public static Func<string, string> FilterFunction;
        public static ConcurrentDictionary<string, string> ColumnMapping;
        public static SqlType OperatorType;
        static SqlModel()
        {
            ColFunction = (item) => { return Column(item); };
        }
        /// <summary>
        /// 根据ColumnAttribute的映射返回实际字段名
        /// </summary>
        /// <param name="item">当前属性</param>
        /// <returns>数据库中实际字段名</returns>
        public static string Column(string item)
        {
            if (ColumnMapping.ContainsKey(item))
            {
                return ColumnMapping[item];
            }
            throw new Exception($"在{typeof(T)}类中找不到属性{item}");
        }
        /// <summary>
        /// 成员集合求并集
        /// </summary>
        /// <param name="members">需要合并的集合</param>
        public static void AddMembers(IEnumerable<string> members)
        {
            if (Members == null)
            {
                throw new NullReferenceException("成员数组不能为空，为空不能做添加操作！");
            }
            Members.UnionWith(members);
        }
        /// <summary>
        /// 加载一个新的集合
        /// </summary>
        /// <param name="members">新集合</param>
        public static void ResetMembers(IEnumerable<string> members)
        {
            Members = null;
            Members = new HashSet<string>(members);
        }
        /// <summary>
        /// 排除集合
        /// </summary>
        /// <param name="ignores">需要排除的集合</param>
        public static void RemoveMembers(IEnumerable<string> ignores)
        {
            if (Members == null)
            {
                throw new NullReferenceException("成员数组不能为空，为空不能做排除操作！");
            }
            Members.ExceptWith(ignores);
        }
        public static SqlModel ModelWithoutPrimary()
        {
            var model = CopyInstance();
            model.RemoveMembers(PrimaryKey);
            return model;
        }
        public static SqlModel CopyInstance()
        {
            return new SqlModel(null)
            {
                Left = Left,
                Right = Right,
                TableName = TableName,
                PrimaryKey = PrimaryKey,
                PrimaryManually = PrimaryManually,
                ColumnMapping = ColumnMapping,
                OperatorType = OperatorType
            };
        }
    }


}
