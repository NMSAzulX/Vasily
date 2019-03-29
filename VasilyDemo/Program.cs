using BenchmarkDotNet.Running;
using System;
using System.Data.SqlClient;
using VasilyDemo.Entities;

namespace VasilyDemo
{
    class Program
    {
        static void Main(string[] args)
        {

            Connector.Add<SqlConnection>("key:sql1", "连接字符串");
            Connector.Add<SqlConnection>("key:sql2", "写-连接字符串", "读-连接字符串");

            DapperWrapper<One> wrapper = new DapperWrapper<One>("key:sql2");

            //重试3次
            //取第1，3次的结果
            int retry_count = 3;
            int[] take_errors = new int[] { 1, 3};

            //准备实体
            One entity = new One();
            entity.age = 100;
            entity.name = "小玉";

            //使用事务
            wrapper.TransactionRetry((read_connection, write_connection) =>
            {
                //wrapper.SafeAdd(check_repeate); Or

                if (!wrapper.IsRepeat(entity))
                {
                    wrapper.Add(entity);
                }
                else
                {
                    //wrapper.ModifyByPrimary(check_repeate);
                    wrapper.Modify(
                        item => item == "age" & item == "name", 
                        entity
                    );
                }
               
            }, retry_count,take_errors);


            wrapper.Gets(item => item > "view" ^ item - "id" ^(10, 5),null);

            //Demo_Sql.Start();

            //Demo_RelationSql.Start();


            //BenchmarkRunner.Run<Demo_Union>();
            Console.ReadKey();
        }
    }
}
