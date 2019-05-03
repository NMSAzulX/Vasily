using Vasily;
using Vasily.Engine;
using Vasily.VP;
using VasilyUT.Entity;
using Xunit;

namespace VasilyUT
{
    public class UnitTest_VasilyUnion
    {
        [Fact(DisplayName = "条件+联合语句测试1")]
        public void TestUnionCondition1()
        {
            SqlMaker<Student> package = new SqlMaker<Student>();
            SqlCondition<Student> condition = new SqlCondition<Student>();
            var result = SqlCollection<Student>.TableUnion(SqlEntity<Student>.SelectAllWhere + (condition > "Sid").Full, "table1", "table2", "table3");
            Assert.Equal("(SELECT * FROM `table1` WHERE `Sid` > @Sid) UNION (SELECT * FROM `table2` WHERE `Sid` > @Sid) UNION (SELECT * FROM `table3` WHERE `Sid` > @Sid)", result);
         }
        [Fact(DisplayName = "条件+联合语句测试2")]
        public void TestUnionCondition2()
        {
            SqlMaker<Student> package = new SqlMaker<Student>();
            SqlCondition<Student> condition = new SqlCondition<Student>();
            var result = SqlCollection<Student>.Union(SqlEntity<Student>.SelectAllWhere + (condition > "Sid").Full);
            Assert.Equal("(SELECT * FROM `1` WHERE `Sid` > @Sid)", result);
        }
        [Fact(DisplayName = "条件+联合语句测试3")]
        public void TestUnionCondition3()
        {
            SqlMaker<Student> package = new SqlMaker<Student>();
            SqlCondition<Student> condition = new SqlCondition<Student>();
            var result = SqlCollection<Student>.TableIntersect(SqlEntity<Student>.SelectAllWhere + (condition > "Sid").Full, "table1", "table2", "table3");
            Assert.Equal("(SELECT * FROM `table1` WHERE `Sid` > @Sid) INTERSECT (SELECT * FROM `table2` WHERE `Sid` > @Sid) INTERSECT (SELECT * FROM `table3` WHERE `Sid` > @Sid)", result);
        }
    }

}
