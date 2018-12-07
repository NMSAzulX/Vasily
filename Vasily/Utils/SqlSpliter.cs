using System;
using System.Collections.Generic;
using System.Text;
using Vasily.Core;

namespace Vasily.Utils
{
    public class SqlSpliter
    {
        public static string GetSpliter(Type type)
        {
            AttrOperator attr = new AttrOperator(type);
            var _sql_type = attr.ClassInstance<TableAttribute>().Type;
            string result = "  ";
            switch (_sql_type)
            {
                case SqlType.MySql:
                    result = "``";
                    break;
                case SqlType.MsSql:
                    result = "[]";
                    break;
                case SqlType.TiDb:
                    break;
                case SqlType.PgSql:
                    break;
                case SqlType.None:
                    break;
                default:
                    break;
            }
            return result;
        }

        public static string GetSpliter(AttrOperator attr)
        {
            var _sql_type = attr.ClassInstance<TableAttribute>().Type;
            string result = "  ";
            switch (_sql_type)
            {
                case SqlType.MySql:
                    result = "``";
                    break;
                case SqlType.MsSql:
                    result = "[]";
                    break;
                case SqlType.TiDb:
                    break;
                case SqlType.PgSql:
                    break;
                case SqlType.None:
                    break;
                default:
                    break;
            }
            return result;
        }
        public static string GetSpliter(SqlType type)
        {
            string result = "  ";
            switch (type)
            {
                case SqlType.MySql:
                    result = "``";
                    break;
                case SqlType.MsSql:
                    result = "[]";
                    break;
                case SqlType.TiDb:
                    break;
                case SqlType.PgSql:
                    break;
                case SqlType.None:
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
