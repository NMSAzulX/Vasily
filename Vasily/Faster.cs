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

        public SqlCP Clone()
        {
            return new SqlCP()
            {
                ConditionWithPage = ConditionWithPage,
                ConditionWithOutPage = ConditionWithOutPage
            };
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
            if (Sql<T>.Cache.ContainsKey(key.sql))
            {
                SqlCP<T> result = (SqlCP<T>)Sql<T>.GetCP(key.sql);
                result.Instance = key.value;
                return result;
            }
            else
            {
                return (SqlCP<T>)(key.value.Condition<T>(key.sql));
            }
        }
    }
}
