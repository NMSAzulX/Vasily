
using Vasily;

namespace System
{
    public static class ObjectExtension
    {
        public static SqlCP Condition(this object instance,SqlCondition condition)
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
            ASTParser<T> parser = new ASTParser<T>();
            return Condition(instance, parser.GetCondition(condition));
        }
    }
}
