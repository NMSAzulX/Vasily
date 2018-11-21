using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Vasily;

namespace System
{

    public class SqlCondition:SqlPBase
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
        public new string Full
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
            PrimaryKey = MakerModel<T>.PrimaryKey;
            Table = MakerModel<T>.TableName;
            OperatorType = MakerModel<T>.OperatorType;
        }
        //public static SqlCondition<T> operator & (SqlCondition<T> field1, string field2)
        //{
        //    if (field1._started)
        //    {
        //        field1.SqlResults.Append(" AND ").Append(field2);
        //    }
        //    else
        //    {
        //        field1.SqlResults.Append(field2);
        //        field1._started = true;
        //    }
        //    return field1;
        //}

        public static SqlCondition<T> operator ^(SqlCondition<T> field1, SqlCondition<T> field2)
        {
            field1.SqlResults.Append(field2.SqlResults);
            if (!field1.HasOrder)
            {
                field1.SqlOrders = field2.SqlOrders;
            }
            return field1;
        }
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
        public static SqlCondition<T> operator &(SqlCondition<T> field1, SqlCondition<T> field2)
        {
            SqlCondition<T> newInstance = new SqlCondition<T>();
            newInstance.SqlOrders = field1.HasOrder ? field1.SqlOrders : field2.SqlOrders;
            newInstance.SqlPages = field1.SqlPages.Length > 0 ? field1.SqlPages : field2.SqlPages;
            //if (newInstance.SqlResults.Length > 0)
            //{
            //    newInstance.SqlResults.Append(" AND ");
            //    newInstance.SqlResults.Append(field2.SqlResults);
            //    newInstance.SqlResults.Append(")");
            //}
            //else
            //{
            newInstance.SqlResults.Append("(");
            newInstance.SqlResults.Append(field1.SqlResults);
            newInstance.SqlResults.Append(" AND ");
            newInstance.SqlResults.Append(field2.SqlResults);
            newInstance.SqlResults.Append(")");
            //}
            return newInstance;
        }

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

        public SqlCondition<T> GetParameterString(string operators, string field2)
        {
            SqlCondition<T> newInstance = new SqlCondition<T>();
            newInstance.SqlResults.Append(MakerModel<T>.Column(field2)).Append($" {operators} @").Append(field2);
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
        public static SqlCondition<T> operator +(SqlCondition<T> field1, string field2)
        {
            if (field1.HasOrder)
            {
                field1.SqlOrders.Append($",{MakerModel<T>.Column(field2)} ASC");
            }
            else
            {
                field1.HasOrder = true;
                field1.SqlOrders.Append($" ORDER BY {MakerModel<T>.Column(field2)} ASC");
            }
            return field1;
        }
        public static SqlCondition<T> operator +(string field2, SqlCondition<T> field1)
        {
            if (field1.HasOrder)
            {
                field1.SqlOrders.Append($",{MakerModel<T>.Column(field2)} ASC");
            }
            else
            {
                field1.HasOrder = true;
                field1.SqlOrders.Append($" ORDER BY {MakerModel<T>.Column(field2)} ASC");
            }
            return field1;
        }
        public static SqlCondition<T> operator -(SqlCondition<T> field1, string field2)
        {
            if (field1.HasOrder)
            {
                field1.SqlOrders.Append($",{MakerModel<T>.Column(field2)} DESC");
            }
            else
            {
                field1.HasOrder = true;
                field1.SqlOrders.Append($" ORDER BY {MakerModel<T>.Column(field2)} DESC");
            }
            return field1;
        }
        public static SqlCondition<T> operator -(string field2, SqlCondition<T> field1)
        {
            if (field1.HasOrder)
            {
                field1.SqlOrders.Append($",{MakerModel<T>.Column(field2)} DESC");
            }
            else
            {
                field1.HasOrder = true;
                field1.SqlOrders.Append($" ORDER BY {MakerModel<T>.Column(field2)} DESC");
            }
            return field1;
        }
        public static SqlCondition<T> operator %(SqlCondition<T> field1, string field2)
        {
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

