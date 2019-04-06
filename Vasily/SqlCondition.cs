using System.Diagnostics;
using System.Text;
using Vasily;

namespace System
{

    public class SqlCondition:SqlConditionBase
    {
        public StringBuilder SqlResults;
        public StringBuilder SqlOrders;
        public StringBuilder SqlPages;
        public bool HasOrder;
        public string PrimaryKey;
        public string Table;
        public SqlType OperatorType;
        public SqlCondition()
        {
            HasOrder = false;
            SqlResults = new StringBuilder(20);
            SqlOrders = new StringBuilder();
            SqlPages = new StringBuilder();
        }
        public void Claer()
        {
            SqlResults.Clear();
            SqlOrders.Clear();
            SqlPages.Clear();
        }

        public override string ToString()
        {
            return Full;
        }
        [Conditional("DEBUG")]
        public void Show(string value)
        {
            Debug.WriteLine(value);
        }

        public string ConditionOrder
        {
            get
            {
                if (SqlResults.Length == 0)
                {
                    SqlResults.Append("1=1");
                }
                return GetString(SqlResults, SqlOrders);
            }
        }

        public string Query
        {
            get
            {
                if (SqlResults.Length == 0)
                {
                    SqlResults.Append("1=1");
                }
                return SqlResults.ToString();
            }
        }
        public string Order { get { return SqlOrders.ToString(); } }
        public string Tails { get { return GetString(SqlOrders, SqlPages); } }
        public override string Full
        {
            get
            {
                if (SqlResults.Length == 0)
                {
                    SqlResults.Append("1=1");
                }
                return GetString(SqlResults, SqlOrders, SqlPages);
            }
        }
        public string GetString(params StringBuilder[] builders)
        {
            int length = 0;
            for (int i = 0; i < builders.Length; i += 1)
            {
                length += builders[i].Length;
            }

            StringBuilder sb = new StringBuilder(length);
            //sb.Append(builders);
            for (int i = 0; i < builders.Length; i += 1)
            {
                sb.Append(builders[i]);
            }
            return sb.ToString();
        }
    }

    public class SqlCondition<T> : SqlCondition
    {
        public SqlCondition()
        {
            PrimaryKey = SqlModel<T>.PrimaryKey;
            Table = SqlModel<T>.TableName;
            OperatorType = SqlModel<T>.OperatorType;
        }

        /// <summary>
        /// 将条件2的SQLResult拼接到条件1中。
        /// 如果条件1没有排序，则使用条件2的排序。
        /// 后置条件向前拼接。
        /// 分页条件保存。
        /// </summary>
        /// <param name="field1">条件实例1</param>
        /// <param name="field2">条件实例2</param>
        /// <returns></returns>
        public static SqlCondition<T> operator ^(SqlCondition<T> field1, SqlCondition<T> field2)
        {
            field1.SqlResults.Append(field2.SqlResults);
            if (!field1.HasOrder)
            {
                field1.SqlOrders = field2.SqlOrders;
            }
            if (field1.SqlPages.Length==0)
            {
                field1.SqlPages = field2.SqlPages;
            }
            return field1;
        }
        /// <summary>
        /// 根据T类型标记的数据库类型，根据类型创建数据库连接，存入条件实例的SqlPage中
        /// </summary>
        /// <param name="field2">分页元祖2</param>
        /// <param name="field1">条件实例1</param>
        /// <returns></returns>
        public static SqlCondition<T> operator ^(SqlCondition<T> field1, (int, int) field2)
        {
            int current_page = field2.Item1;
            int size = field2.Item2;
            switch (field1.OperatorType)
            {
                case SqlType.MySql:
                    //field1.SqlPages.Append($" {field1.PrimaryKey} >=(SELECT {field1.PrimaryKey} FROM {field1.Table} limit {(current_page - 1) * size},1)").Append(field1.mysql_temp_unique).Append($" limit {size}");
                    field1.SqlPages.Append($" LIMIT {(current_page - 1) * size},{size}");
                    break;
                case SqlType.MsSql:
                    field1.SqlPages.Append($" OFFSET {size * (current_page - 1)} ROW FETCH NEXT {size} rows only");
                    break;
                case SqlType.TiDb:
                    break;
                case SqlType.PgSql:
                case SqlType.SqlLite:
                    field1.SqlPages.Append($" LIMIT {size} OFFSET {size * (current_page - 1)}");
                    break;
                default:
                    break;
            }
            return field1;
        }
        /// <summary>
        /// 根据T类型标记的数据库类型，根据类型创建数据库连接，存入条件实例SqlPage中
        /// </summary>
        /// <param name="field2">分页元祖2</param>
        /// <param name="field1">条件实例1</param>
        /// <returns></returns>
        public static SqlCondition<T> operator ^((int, int) field2, SqlCondition<T> field1)
        {
            int current_page = field2.Item1;
            int size = field2.Item2;
            switch (field1.OperatorType)
            {
                case SqlType.MySql:
                    //field1.SqlPages.Append($" {field1.PrimaryKey} >=(SELECT {field1.PrimaryKey} FROM {field1.Table} limit {(current_page - 1) * size},1)").Append(field1.mysql_temp_unique).Append($" limit {size}");
                    field1.SqlPages.Append($" LIMIT {(current_page - 1) * size},{size}");
                    break;
                case SqlType.MsSql:
                    field1.SqlPages.Append($" OFFSET {size * (current_page - 1)} ROW FETCH NEXT {size} rows only");
                    break;
                case SqlType.TiDb:
                    break;
                case SqlType.PgSql:
                case SqlType.SqlLite:
                    field1.SqlPages.Append($" LIMIT {size} OFFSET {size * (current_page - 1)}");
                    break;
                default:
                    break;
            }
            return field1;
        }

        /// <summary>
        /// 将两个查询条件进行拼接，(条件1 AND 条件2).
        /// 顺序查询，分页查询将融合到新的实例中等待处理
        /// </summary>
        /// <param name="field1">条件实例1</param>
        /// <param name="field2">条件实例2</param>
        public static SqlCondition<T> operator &(SqlCondition<T> field1, SqlCondition<T> field2)
        {
            SqlCondition<T> newInstance = new SqlCondition<T>();
            newInstance.SqlOrders = field1.HasOrder ? field1.SqlOrders : field2.SqlOrders;
            newInstance.SqlPages = field1.SqlPages.Length > 0 ? field1.SqlPages : field2.SqlPages;
            newInstance.SqlResults.Append("(");
            newInstance.SqlResults.Append(field1.SqlResults);
            newInstance.SqlResults.Append(" AND ");
            newInstance.SqlResults.Append(field2.SqlResults);
            newInstance.SqlResults.Append(")");
            //}
            return newInstance;
        }


        /// <summary>
        /// 将两个查询条件进行拼接，(条件1 OR 条件2).
        /// 保存排序。
        /// 保存分页。
        /// </summary>
        /// <param name="field1">条件实例1</param>
        /// <param name="field2">条件实例2</param>
        /// <returns></returns>
        public static SqlCondition<T> operator |(SqlCondition<T> field1, SqlCondition<T> field2)
        {
            SqlCondition<T> newInstance = new SqlCondition<T>();
            newInstance.SqlOrders = field1.HasOrder ? field1.SqlOrders : field2.SqlOrders;
            newInstance.SqlPages = field1.SqlPages.Length > 0 ? field1.SqlPages : field2.SqlPages;

            newInstance.SqlResults.Append("(");
            newInstance.SqlResults.Append(field1.SqlResults);
            newInstance.SqlResults.Append(" OR ");
            newInstance.SqlResults.Append(field2.SqlResults);
            newInstance.SqlResults.Append(")");
            return newInstance;
        }

        /// <summary>
        /// 根据字段以及操作符进行组合，[真实字段] 操作符 @[字段]
        /// </summary>
        /// <param name="operators">操作符 =<>!= </param>
        /// <param name="field2">数据库字段名 name/age/createtime 等</param>
        /// <returns></returns>
        public SqlCondition<T> GetParameterString(string operators, string field2)
        {
            SqlCondition<T> newInstance = new SqlCondition<T>();
            newInstance.SqlResults.Append($"{SqlModel<T>.Left}{SqlModel<T>.Column(field2)}{SqlModel<T>.Right}").Append($" {operators} @").Append(field2);
            return newInstance;
        }


        public static SqlCondition<T> operator >=(SqlCondition<T> field1, string field2)
        {
            return field1.GetParameterString(">=", field2);
        }
        public static SqlCondition<T> operator <=(SqlCondition<T> field1, string field2)
        {
            return field1.GetParameterString("<=", field2);
        }
        public static SqlCondition<T> operator >=(string field2, SqlCondition<T> field1)
        {
            return field1.GetParameterString(">=", field2);
        }
        public static SqlCondition<T> operator <=(string field2, SqlCondition<T> field1)
        {
            return field1.GetParameterString("<=", field2);
        }
        public static SqlCondition<T> operator ==(SqlCondition<T> field1, string field2)
        {
            return field1.GetParameterString("=", field2);
        }
        public static SqlCondition<T> operator ==(string field2, SqlCondition<T> field1)
        {
            return field1.GetParameterString("=", field2);
        }
        public static SqlCondition<T> operator !=(SqlCondition<T> field1, string field2)
        {
            return field1.GetParameterString("<>", field2);
        }
        public static SqlCondition<T> operator !=(string field2, SqlCondition<T> field1)
        {
            return field1.GetParameterString("<>", field2);
        }
        public static SqlCondition<T> operator >(SqlCondition<T> field1, string field2)
        {
            return field1.GetParameterString(">", field2);
        }
        public static SqlCondition<T> operator <(SqlCondition<T> field1, string field2)
        {
            return field1.GetParameterString("<", field2);
        }
        public static SqlCondition<T> operator >(string field2, SqlCondition<T> field1)
        {
            return field1.GetParameterString(">", field2);
        }
        public static SqlCondition<T> operator <(string field2, SqlCondition<T> field1)
        {
            return field1.GetParameterString("<", field2);
        }

        /// <summary>
        /// 给条件实例的SqlOrders缓存增加升序
        /// string参数为SQL字段，会被翻译成真实字段
        /// </summary>
        /// <param name="field1">条件实例</param>
        /// <param name="field2">字段</param>
        /// <returns></returns>
        public static SqlCondition<T> operator +(SqlCondition<T> field1, string field2)
        {
            if (field1.HasOrder)
            {
                field1.SqlOrders.Append($",{SqlModel<T>.Left}{SqlModel<T>.Column(field2)}{SqlModel<T>.Right} ASC");
            }
            else
            {
                field1.HasOrder = true;
                field1.SqlOrders.Append($" ORDER BY {SqlModel<T>.Left}{SqlModel<T>.Column(field2)}{SqlModel<T>.Right} ASC");
            }
            return field1;
        }
        public static SqlCondition<T> operator +(string field2, SqlCondition<T> field1)
        {
            if (field1.HasOrder)
            {
                field1.SqlOrders.Append($",{SqlModel<T>.Left}{SqlModel<T>.Column(field2)}{SqlModel<T>.Right} ASC");
            }
            else
            {
                field1.HasOrder = true;
                field1.SqlOrders.Append($" ORDER BY {SqlModel<T>.Left}{SqlModel<T>.Column(field2)}{SqlModel<T>.Right} ASC");
            }
            return field1;
        }
        public static SqlCondition<T> operator -(SqlCondition<T> field1, string field2)
        {
            if (field1.HasOrder)
            {
                field1.SqlOrders.Append($",{SqlModel<T>.Left}{SqlModel<T>.Column(field2)}{SqlModel<T>.Right} DESC");
            }
            else
            {
                field1.HasOrder = true;
                field1.SqlOrders.Append($" ORDER BY {SqlModel<T>.Left}{SqlModel<T>.Column(field2)}{SqlModel<T>.Right} DESC");
            }
            return field1;
        }
        public static SqlCondition<T> operator -(string field2, SqlCondition<T> field1)
        {
            if (field1.HasOrder)
            {
                field1.SqlOrders.Append($",{SqlModel<T>.Left}{SqlModel<T>.Column(field2)}{SqlModel<T>.Right} DESC");
            }
            else
            {
                field1.HasOrder = true;
                field1.SqlOrders.Append($" ORDER BY {SqlModel<T>.Left}{SqlModel<T>.Column(field2)}{SqlModel<T>.Right} DESC");
            }
            return field1;
        }

        public static SqlCondition<T> operator %(SqlCondition<T> field1, string field2)
        {
            //$"{SqlModel<T>.Left}{SqlModel<T>.Column(field2)}{SqlModel<T>.Right}"
            return field1.GetParameterString("LIKE", field2);
        }
        public static SqlCondition<T> operator %(string field2, SqlCondition<T> field1)
        {
            return field1.GetParameterString("LIKE", field2);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }




}

