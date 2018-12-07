using System;
using System.Collections.Generic;
using System.Text;

namespace Vasily
{
    public class SqlCollection
    {
        private static string Contact(string operate, params string[] sqls)
        {
            if (sqls.Length == 0)
            {
                return null;
            }
            StringBuilder result = new StringBuilder((sqls[0].Length + 2) * sqls.Length);
            for (int i = 0; i < sqls.Length - 1; i += 1)
            {
                result.Append('(').Append(sqls[i]).Append($") {operate} ");
            }
            result.Append('(').Append(sqls[sqls.Length-1]).Append(')');
            return result.ToString();
        }

        private static string Contact(string operate, string source_table, string source_sql, params string[] tables)
        {
            if (tables.Length == 0)
            {
                return source_sql;
            }
            StringBuilder result = new StringBuilder((source_sql.Length + 2) * tables.Length);
            for (int i = 0; i < tables.Length - 1; i += 1)
            {
                result.Append('(').Append(source_sql.Replace(source_table, tables[i])).Append($") {operate} ");
            }
            result.Append('(').Append(source_sql.Replace(source_table, tables[tables.Length - 1])).Append(')');
            return result.ToString();
        }
        public static string Union(params string[] sqls)
        {
            return Contact("UNION", sqls);
        }
        public static string UnionAll(params string[] sqls)
        {
            return Contact("UNION ALL", sqls);
        }
        public static string Except(params string[] sqls)
        {
            return Contact("EXCEPT", sqls);
        }
        public static string Intersect(params string[] sqls)
        {
            return Contact("INTERSECT", sqls);
        }

        public static string TableUnion(string source_table, string source_sql, params string[] tables)
        {
            return Contact("UNION", source_table, source_sql, tables);
        }
        public static string TableUnionAll(string source_table, string source_sql, params string[] tables)
        {
            return Contact("UNION ALL", source_table, source_sql, tables);
        }
        public static string TableExcept(string source_table, string source_sql, params string[] tables)
        {
            return Contact("EXCEPT", source_table, source_sql, tables);
        }
        public static string TableIntersect(string source_table, string source_sql, params string[] tables)
        {
            return Contact("INTERSECT", source_table, source_sql, tables);
        }
        public static string Collection(SqlCollectionType type, string source_table, string source_sql, params string[] tables)
        {
            switch (type)
            {

                case SqlCollectionType.Union:
                    return TableUnion(source_table, source_sql, tables);
                case SqlCollectionType.UnionAll:
                    return TableUnionAll(source_table, source_sql, tables);
                case SqlCollectionType.Except:
                    return TableExcept(source_table, source_sql, tables);
                case SqlCollectionType.Intersect:
                    return TableIntersect(source_table, source_sql, tables);
                case SqlCollectionType.None:
                default:
                    return source_sql;
            }
        }
        public static string Collection(SqlCollectionType type, params string[] sqls)
        {
            switch (type)
            {
                case SqlCollectionType.None:
                case SqlCollectionType.Union:
                default:
                    return Union(sqls);
                case SqlCollectionType.UnionAll:
                    return UnionAll(sqls);
                case SqlCollectionType.Except:
                    return Except(sqls);
                case SqlCollectionType.Intersect:
                    return Intersect(sqls);
            }
        }
    }

    public class SqlCollection<T> : SqlCollection
    {

        public static string TableUnion(string source_sql, params string[] tables)
        {
            return TableUnion(SqlModel<T>.TableName, source_sql, tables);
        }
        public static string TableUnionAll(string source_sql, params string[] tables)
        {
            return TableUnionAll(SqlModel<T>.TableName, source_sql, tables);
        }
        public static string TableExcept(string source_sql, params string[] tables)
        {
            return TableExcept(SqlModel<T>.TableName, source_sql, tables);
        }
        public static string TableIntersect(string source_sql, params string[] tables)
        {
            return TableIntersect(SqlModel<T>.TableName, source_sql, tables);
        }
        public static string Collection(SqlCollectionType type, string source_sql, params string[] tables)
        {
            switch (type)
            {

                case SqlCollectionType.Union:
                    return TableUnion(source_sql, tables);
                case SqlCollectionType.UnionAll:
                    return TableUnionAll(source_sql, tables);
                case SqlCollectionType.Except:
                    return TableExcept(source_sql, tables);
                case SqlCollectionType.Intersect:
                    return TableIntersect(source_sql, tables);
                case SqlCollectionType.None:
                default:
                    return source_sql;
            }
        }
    }

    public enum SqlCollectionType
    {
        None,
        Union,
        UnionAll,
        Except,
        Intersect
    }
}
