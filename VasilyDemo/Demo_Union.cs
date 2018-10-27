using System;
using System.Collections.Generic;
using System.Text;
using Vasily;
using VasilyDemo.Entities;

namespace VasilyDemo
{
    public class Demo_Union
    {
        SqlCondition<One> condition;
        public Demo_Union()
        {
            SqlPackage<One> package = new SqlPackage<One>();
            condition = new SqlCondition<One>();

        }
        [BenchmarkDotNet.Attributes.Benchmark]
        public void TestUnion()
        {
            var result = SqlUnion<One>.Union(Sql<One>.SelectAllByCondition + (condition > "oid").Full, "table1", "table2", "table3");
        }
        [BenchmarkDotNet.Attributes.Benchmark]
        public void TestFull()
        {
            var result = Sql<One>.SelectAllByCondition + (condition > "oid").Full;
        }
        [BenchmarkDotNet.Attributes.Benchmark]
        public void TestConditon()
        {
            var result =(condition > "oid").Full;
        }
        [BenchmarkDotNet.Attributes.Benchmark]
        public void TestScript()
        {
            string test = "c<=oid&c==name|c!= create_time^c+oid- create_time^(3,10)";
            string result = test.Condition<One>(test).Full;
        }
        [BenchmarkDotNet.Attributes.Benchmark]
        public void TestScriptCache()
        {
            string test = "c!= create_time^c+oid- create_time^(3,10)";
            string result = test.Condition<One>(test).Full;
            string cache = test.Condition<One>(test).Full;
            cache = test.Condition<One>(test).Full;
            cache = test.Condition<One>(test).Full;
        }
    }
}
