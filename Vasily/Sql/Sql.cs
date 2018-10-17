using Vasily.Core;

namespace Vasily
{
    
    public class Sql<T>
    {

        public static MemberSetter SetPrimary;
        public static string Primary;
        public static string Table;

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
