using System;
using System.Collections.Generic;
using System.Text;
using Vasily;
using VasilyUT.Entity;
using Xunit;

namespace VasilyUT
{
    public class UnitTest_VasilyUnion
    {
        [Fact(DisplayName = "条件+联合语句测试1")]
        public void TestUnionCondition1()
        {
            SqlPackage<Student> package = new SqlPackage<Student>();
            SqlCondition<Student> condition = new SqlCondition<Student>();
            var result = SqlUnion<Student>.Union(Sql<Student>.SelectAllByCondition + (condition > "Sid").Full, "table1", "table2", "table3");
            Assert.Equal("SELECT * FROM `table1` WHERE Sid > @Sid UNION SELECT * FROM `table2` WHERE Sid > @Sid UNION SELECT * FROM `table3` WHERE Sid > @Sid", result);
         }
        [Fact(DisplayName = "条件+联合语句测试2")]
        public void TestUnionCondition2()
        {
            SqlPackage<Student> package = new SqlPackage<Student>();
            SqlCondition<Student> condition = new SqlCondition<Student>();
            var result = SqlUnion<Student>.Union(Sql<Student>.SelectAllByCondition + (condition > "Sid").Full);
            Assert.Equal("SELECT * FROM `1` WHERE Sid > @Sid", result);
        }
    }

}
