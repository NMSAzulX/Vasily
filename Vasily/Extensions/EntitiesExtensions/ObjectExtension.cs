
namespace System
{
    public static class ObjectExtension
    {
        public static SqlCP Condition(this object instance,SqlCondition condition)
        {
            SqlCP cp = default(SqlCP);
            cp.Instance = instance;
            cp.ConditionWithOutPage = condition.GetConditionWithoutPage();
            cp.ConditionWithPage = condition.GetConditionWithPage();
            condition.Claer();
            return cp;
        }
    }
}
