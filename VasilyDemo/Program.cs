using BenchmarkDotNet.Running;
using System;

namespace VasilyDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Demo_Sql.Start();
            //Demo_RelationSql.Start();
            BenchmarkRunner.Run<Demo_Union>();
            Console.ReadKey();
        }
    }
}
