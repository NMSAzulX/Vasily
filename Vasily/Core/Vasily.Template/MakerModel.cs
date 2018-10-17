using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Vasily
{
    public class MakerModel
    {
        public char Left;
        public char Right;
        public string TableName;
        public string PrimaryKey;
        public HashSet<MemberInfo> Members;
        public Func<MemberInfo, string> ColFunction;
        public Func<MemberInfo, string> FilterFunction;
        public Dictionary<MemberInfo, string> ColumnMapping;

        public MakerModel()
        {

        }
        /// <summary>
        /// 根据ColumnAttribute的映射返回实际字段名
        /// </summary>
        /// <param name="item">当前属性</param>
        /// <returns>数据库中实际字段名</returns>
        public string Column(MemberInfo item)
        {
            if (ColumnMapping.ContainsKey(item))
            {
                return ColumnMapping[item];
            }
            return item.Name;
        }
        /// <summary>
        /// 成员集合求并集
        /// </summary>
        /// <param name="members">需要合并的集合</param>
        public void AddMembers(IEnumerable<MemberInfo> members)
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
        public void LoadMembers(params MemberInfo[] members)
        {
            Members = null;
            Members = new HashSet<MemberInfo>(members);
        }
        public void LoadMembers(IEnumerable<MemberInfo> members)
        {
            Members = null;
            Members = new HashSet<MemberInfo>(members);
        }
        /// <summary>
        /// 排除集合
        /// </summary>
        /// <param name="ignores">需要排除的集合</param>
        public void AddIgnores(IEnumerable<MemberInfo> ignores)
        {
            if (Members == null)
            {
                throw new NullReferenceException("成员数组不能为空，为空不能做排除操作！");
            }
            Members.ExceptWith(ignores);
        }
        public void AddIgnores(params MemberInfo[] ignores)
        {
            if (Members == null)
            {
                throw new NullReferenceException("成员数组不能为空，为空不能做排除操作！");
            }
            Members.ExceptWith(ignores);
        }
        public MakerModel CopyInstance()
        {
            MakerModel newModel = new MakerModel()
            {
                Left = Left,
                Right = Right,
                TableName = TableName,
                PrimaryKey = PrimaryKey,
            };
            newModel.Members = new HashSet<MemberInfo>(Members);
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
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class MakerModel<T>
    {
        public static char Left;
        public static char Right;
        public static string TableName;
        public static string PrimaryKey;
        public static HashSet<MemberInfo> Members;
        public static Func<MemberInfo, string> ColFunction;
        public static Func<MemberInfo, string> FilterFunction;
        public static Dictionary<MemberInfo, string> ColumnMapping;

        static MakerModel()
        {

        }
        /// <summary>
        /// 根据ColumnAttribute的映射返回实际字段名
        /// </summary>
        /// <param name="item">当前属性</param>
        /// <returns>数据库中实际字段名</returns>
        public static string Column(MemberInfo item)
        {
            if (ColumnMapping.ContainsKey(item))
            {
                return ColumnMapping[item];
            }
            return item.Name;
        }
        /// <summary>
        /// 成员集合求并集
        /// </summary>
        /// <param name="members">需要合并的集合</param>
        public static void AddMembers(IEnumerable<MemberInfo> members)
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
        public static void LoadMembers(IEnumerable<MemberInfo> members)
        {
            Members = null;
            Members = new HashSet<MemberInfo>(members);
        }
        /// <summary>
        /// 排除集合
        /// </summary>
        /// <param name="ignores">需要排除的集合</param>
        public static void AddIgnores(IEnumerable<MemberInfo> ignores)
        {
            if (Members == null)
            {
                throw new NullReferenceException("成员数组不能为空，为空不能做排除操作！");
            }
            Members.ExceptWith(ignores);
        }

        public static MakerModel CopyInstance()
        {
            return new MakerModel()
            {
                Left = Left,
                Right = Right,
                TableName = TableName,
                PrimaryKey = PrimaryKey,
                Members = Members,
            };
        }
    }
}
