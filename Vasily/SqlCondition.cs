using System.Collections.Generic;
using System.Text;
using Vasily;

namespace System
{
    public class SqlCondition<T>
    {
        public StringBuilder SqlResults;

        public SqlCondition()
        {
            SqlResults = new StringBuilder();
        }
        public void Clear()
        {
            SqlResults.Clear();
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
        public static SqlCondition<T> operator &(SqlCondition<T> field1, SqlCondition<T> field2)
        {
            SqlCondition<T> newInstance = new SqlCondition<T>();
            if (newInstance.SqlResults.Length>0)
            {
                newInstance.SqlResults.Append(" AND ");
                newInstance.SqlResults.Append(field2.SqlResults);
                newInstance.SqlResults.Append(")");
            }
            else
            {
                newInstance.SqlResults.Append("(");
                newInstance.SqlResults.Append(field1.SqlResults);
                newInstance.SqlResults.Append(" AND ");
                newInstance.SqlResults.Append(field2.SqlResults);
                newInstance.SqlResults.Append(")");
            }
            return newInstance;
        }
        public static SqlCondition<T> operator |(SqlCondition<T> field1, SqlCondition<T> field2)
        {
            SqlCondition<T> newInstance = new SqlCondition<T>();
            if (newInstance.SqlResults.Length > 0)
            {
                newInstance.SqlResults.Append(" OR ");
                newInstance.SqlResults.Append(field2.SqlResults);
                newInstance.SqlResults.Append(")");
            }
            else
            {
                newInstance.SqlResults.Append("(");
                newInstance.SqlResults.Append(field1.SqlResults);
                newInstance.SqlResults.Append(" OR ");
                newInstance.SqlResults.Append(field2.SqlResults);
                newInstance.SqlResults.Append(")");
            }
            return newInstance;
        }
        //public static SqlCondition<T> operator &(string field2,SqlCondition<T> field1)
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
        //public static SqlCondition<T> operator |(SqlCondition<T> field1, string field2)
        //{
        //    if (field1._started)
        //    {
        //        field1.SqlResults.Append(" OR ").Append(field2);
        //    }
        //    else
        //    {
        //        field1.SqlResults.Append(field2);
        //        field1._started = true;
        //    }
        //    return field1;
        //}
        //public static SqlCondition<T> operator |(string field2,SqlCondition<T> field1)
        //{
        //    if (field1._started)
        //    {
        //        field1.SqlResults.Append(" OR ").Append(field2);
        //    }
        //    else
        //    {
        //        field1.SqlResults.Append(field2);
        //        field1._started = true;
        //    }
        //    if (field1._temp != null)
        //    {
        //        field1.SqlResults.Append(field1._temp);
        //    }
        //    return field1;
        //}
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
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
           string result = SqlResults.ToString();
            SqlResults.Clear();
            return result;
        }
    }
}
