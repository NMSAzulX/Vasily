using Vasily;

namespace System
{
    public class SqlVP<T>
    {
        public T value { get; set; }
        public string sql { get; set; }

    }

    public class SqlCP
    {
        public object Instance;
        public string ConditionWithPage;
        public string ConditionWithOutPage;

        public override string ToString()
        {
            return ConditionWithPage;
        }

    }

    public class SqlCP<T> : SqlCP
    {
        public override string ToString()
        {
            return ConditionWithPage;
        }
        public static implicit operator SqlCP<T>(SqlVP<T> key)
        {
            ASTParser<T> parser = new ASTParser<T>();
            var condition = parser.GetCondition(key.sql);
            SqlCP<T> cp = new SqlCP<T>();
            cp.Instance = key.value;
            cp.ConditionWithOutPage = condition.GetConditionWithoutPage();
            cp.ConditionWithPage = condition.GetConditionWithPage();
            condition.Claer();
            return cp;
        }
    }
}
