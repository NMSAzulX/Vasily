namespace System
{
    public static class ObjectExtension
    {
        public static SqlCP Condition(this object instance, SqlCondition condition)
        {
            SqlCP cp = new SqlCP();
            cp.Instance = instance;
            cp.ConditionWithOutPage = condition.GetConditionWithoutPage();
            cp.ConditionWithPage = condition.GetConditionWithPage();
            condition.Claer();
            return cp;
        }
        public static SqlCP Condition<T>(this object instance, string condition)
        {
            var result = Sql<T>.GetCP(condition);
            result.Instance = instance;
            return result;
        }
    }
}
