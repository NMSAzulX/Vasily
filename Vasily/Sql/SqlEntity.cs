using System.Collections.Concurrent;
using Vasily;
using Vasily.Core;

namespace System
{
    
    public class SqlEntity<T>
    {
        static SqlEntity()
        {

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
