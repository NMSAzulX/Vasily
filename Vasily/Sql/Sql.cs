using System.Collections.Concurrent;
using Vasily;
using Vasily.Core;

namespace System
{
    
    public class Sql<T>
    {
        public static ConcurrentDictionary<string, SqlCP> Cache;
        static Sql()
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
        public static string SelectCountByCondition;

        public static string SelectAll;
        public static string SelectAllByCondition;
        public static string SelectAllByPrimary;
        public static string SelectAllIn;

        public static string Select;
        public static string SelectByCondition;
        public static string SelectByPrimary;
        public static string SelectIn;

        public static string UpdateByCondition;
        public static string UpdateByPrimary;

        public static string UpdateAllByCondition;
        public static string UpdateAllByPrimary;

        public static string DeleteByCondition;
        public static string DeleteByPrimary;

        public static string InsertAll;
        public static string Insert;

        public static string RepeateCount;
        public static string RepeateId;
        public static string RepeateEntities;
    }
}
