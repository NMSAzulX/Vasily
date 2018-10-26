using Vasily;

namespace System
{
    public class SqlVP {
        public object value { get; set; }
        public string sql { get; set; }
    }

    public class SqlVP<T>: SqlVP
    {
        public new T value { get; set; }
    }

    public class SqlCP
    {
        public object Instance;
        public string Condition;
        public string Order;
        public string Tails;

        public string Full { get { return Condition + Tails; } }
        public override string ToString()
        {
            return Condition+Tails;
        }

        public SqlCP Clone()
        {
            return new SqlCP()
            {
                Condition = Condition,
                Order = Order,
                Tails = Tails,
                Instance = Instance
            };
        }

        public SqlCP<T> Clone<T>()
        {
            return new SqlCP<T>()
            {
                Condition = Condition,
                Order = Order,
                Tails = Tails,
                Instance = Instance
            };
        }

    }
    public class SqlCP<T> : SqlCP
    {
        public static implicit operator SqlCP<T>(SqlVP<T> key)
        {
            if (Sql<T>.Cache.ContainsKey(key.sql))
            {
                SqlCP<T> result = Sql<T>.GetCP(key.sql).Clone<T>();
                result.Instance = key.value;
                return result;
            }
            else
            {
                return (key.value.Condition<T>(key.sql)).Clone<T>();
            }
        }
    }
}
