using System.Collections.Concurrent;
using Vasily;
using Vasily.Core;

namespace System
{
    
    public class SqlEntity<T>
    {
        public static ConcurrentDictionary<string, SqlCP> Cache;
        static SqlEntity()
        {
            Cache = new ConcurrentDictionary<string, SqlCP>();
        }
        public static SqlCP GetCP(string script)
        {
            if (Cache.ContainsKey(script))
            {
                return Cache[script].Clone();
            }
            else
            {
                ASTParser<T> parser = new ASTParser<T>();
                SqlCP cp = new object().Condition(parser.GetCondition(script));
                Cache[script] = cp;
                return cp;
            }
        }

        public static MemberSetter SetPrimary;
        public static string Primary;
        public static string Table;

        public static string SelectCount;
        public static string SelectCountWhere;

        public static string SelectAll;
        public static string SelectAllWhere;
        public static string SelectAllByPrimary;
        public static string SelectAllIn;

        public static string Select;
        public static string SelectWhere;
        public static string SelectByPrimary;
        public static string SelectIn;

        public static string UpdateWhere;
        public static string UpdateByPrimary;

        public static string UpdateAllWhere;
        public static string UpdateAllByPrimary;

        public static string DeleteWhere;
        public static string DeleteByPrimary;

        public static string InsertAll;
        public static string Insert;

        public static string RepeateCount;
        public static string RepeateId;
        public static string RepeateEntities;
    }
}
