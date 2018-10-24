using System;

namespace Vasily
{
    public class TableAttribute : Attribute
    {
        public static SqlType InitSqlType;
        static TableAttribute()
        {
            InitSqlType = SqlType.None;
        }

        public SqlType Type;
        public string Name;

        public TableAttribute(string tableName, SqlType sqlType = SqlType.None)
        {
            Name = tableName;
            if (sqlType == SqlType.None)
            {
                sqlType = InitSqlType;
            }
            Type = sqlType;
        }
        
    }

    public enum SqlType
    {
        MySql,
        MsSql,
        TiDb,
        PgSql,
        SqlLite,
        None
    }
}
