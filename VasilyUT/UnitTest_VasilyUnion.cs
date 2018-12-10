using System;
using System.Collections.Generic;
using System.Text;
using Vasily;
using Vasily.Core;
using VasilyUT.Entity;
using Xunit;

namespace VasilyUT
{
    public class UnitTest_VasilyUnion
    {
        [Fact(DisplayName = "条件+联合语句测试1")]
        public void TestUnionCondition1()
        {
            NormalAnalysis<Student> package = new NormalAnalysis<Student>();
            SqlCondition<Student> condition = new SqlCondition<Student>();
            var result = SqlCollection<Student>.TableUnion(SqlEntity<Student>.SelectAllWhere + (condition > "Sid").Full, "table1", "table2", "table3");
            Assert.Equal("(SELECT * FROM `table1` WHERE Sid > @Sid) UNION (SELECT * FROM `table2` WHERE Sid > @Sid) UNION (SELECT * FROM `table3` WHERE Sid > @Sid)", result);
         }
        [Fact(DisplayName = "条件+联合语句测试2")]
        public void TestUnionCondition2()
        {
            NormalAnalysis<Student> package = new NormalAnalysis<Student>();
            SqlCondition<Student> condition = new SqlCondition<Student>();
            var result = SqlCollection<Student>.Union(SqlEntity<Student>.SelectAllWhere + (condition > "Sid").Full);
            Assert.Equal("(SELECT * FROM `1` WHERE Sid > @Sid)", result);
        }
        [Fact(DisplayName = "条件+联合语句测试3")]
        public void TestUnionCondition3()
        {
            NormalAnalysis<Student> package = new NormalAnalysis<Student>();
            SqlCondition<Student> condition = new SqlCondition<Student>();
            var result = SqlCollection<Student>.TableIntersect(SqlEntity<Student>.SelectAllWhere + (condition > "Sid").Full, "table1", "table2", "table3");
            Assert.Equal("(SELECT * FROM `table1` WHERE Sid > @Sid) INTERSECT (SELECT * FROM `table2` WHERE Sid > @Sid) INTERSECT (SELECT * FROM `table3` WHERE Sid > @Sid)", result);
        }
    }

}
