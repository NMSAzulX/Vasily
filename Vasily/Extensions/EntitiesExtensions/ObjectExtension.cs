
namespace System
{
    public static class ObjectExtension
    {
        public static SqlCP Condition(this object instance,SqlCondition condition)
        {
            SqlCP cp = default(SqlCP);
            cp.sql = condition.ToString();
            cp.instance = instance;
            return cp;
        }
    }
}
