using System;
using System.Collections.Concurrent;
using Vasily.Model;

namespace Vasily
{
    public class VasilyProtocal<T> : SqlConditionBase
    {
        public static ConcurrentDictionary<string, VasilyProtocal<T>> Cache;
        static VasilyProtocal()
        {
            Cache = new ConcurrentDictionary<string, VasilyProtocal<T>>();
        }

        public VasilyProtocal()
        {

        }
        public VasilyProtocal(string script)
        {
            Script = script;
        }
        public T AcceptInstance
        {
            set
            {
                Instance = value;
            }
        }

        /// <summary>
        /// /Protocal SET Instance
        /// </summary>
        public object Instance;
        /// <summary>
        /// Protocal SET Script
        /// </summary>
        public string Script
        {
            set
            {
                if (!Cache.ContainsKey(value))
                {
                    ASTParser<T> parser = new ASTParser<T>();
                    var condition = parser.GetCondition(value);
                    Query = condition.Query;
                    Tails = condition.Tails;
                    Order = condition.Order;
                    Cache[value] = this;
                }
                else
                {
                    var temp = Cache[value];
                    Query = temp.Query;
                    Tails = temp.Tails;
                    Order = temp.Order;
                }
            }
        }


        public string Query;
        public string Order;
        public string Tails;

        public override string Full { get { return Query + Tails; } }

        public string[] Fields;

        public override string ToString()
        {
            return Query + Tails;
        }
        public VasilyProtocal<T> Clone()
        {
            return new VasilyProtocal<T>()
            {
                Query = Query,
                Order = Order,
                Tails = Tails
            };
        }

        //public static implicit operator VasilyProtocal<T>(T value)
        //{
        //    return new VasilyProtocal<T>() { Instance = value };
        //}
        public static implicit operator VasilyProtocal<T>(string value)
        {
            return new VasilyProtocal<T>(value);
        }
        public static implicit operator VasilyProtocal<T>(SqlCondition<T> value)
        {
            var result = new VasilyProtocal<T>();
            result.Order = value.Order;
            result.Query = value.Query;
            result.Tails = value.Tails;
            return result;
        }
        public static implicit operator VasilyProtocal<T>(Func<SqlCondition<T>, SqlCondition<T>> func)
        {
            var result = new VasilyProtocal<T>();
            var value = func(new SqlCondition<T>());
            result.Order = value.Order;
            result.Query = value.Query;
            result.Tails = value.Tails;
            return result;
        }
    }

    public class VasilyProtocal<T, S> : VasilyProtocal<T> {
        public new S Instance;
    }

}
