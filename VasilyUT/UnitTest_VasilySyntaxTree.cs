using System;
using System.Collections.Generic;
using System.Text;
using Vasily;
using VasilyUT.Entity;
using Xunit;

namespace VasilyUT
{
    public class UnitTest_VasilySyntaxTree
    {

        [Fact(DisplayName = "语法树-优先级解析1")]
        public void TestNormalScript()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            string test = "c<=Id&c==StudentName|c!= ClassId^c+Id- ClassId^(3,10)";
            Assert.Equal("((Id <= @Id AND StudentName = @StudentName) OR ClassId <> @ClassId) ORDER BY Id ASC,ClassId DESC OFFSET 20 ROW FETCH NEXT 10 rows only", 
                test.Condition<Relation2>(test).Full);
        }

        [Fact(DisplayName = "语法树-优先级解析2")]
        public void TestNormalScript1()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            string test = "c<=Id&(c==StudentName|c!= ClassId)^c+Id-ClassId^(3,10)";
            Assert.Equal("(Id <= @Id AND (StudentName = @StudentName OR ClassId <> @ClassId)) ORDER BY Id ASC,ClassId DESC OFFSET 20 ROW FETCH NEXT 10 rows only",
                test.Condition<Relation2>(test).Full);
        }
        [Fact(DisplayName = "语法树-乱序解析1")]
        public void TestNormalScript2()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            string test = "c+Id-ClassId^(3,10) ^c<=Id&(c==StudentName|c!= ClassId)";
            Assert.Equal("(Id <= @Id AND (StudentName = @StudentName OR ClassId <> @ClassId)) ORDER BY Id ASC,ClassId DESC OFFSET 20 ROW FETCH NEXT 10 rows only",
                test.Condition<Relation2>(test).Full);
        }
        [Fact(DisplayName = "语法树-乱序解析2")]
        public void TestNormalScript3()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            string test = "c+Id-ClassId ^c<=Id&(c==StudentName|c!= ClassId)^(3,10)";
            Assert.Equal("(Id <= @Id AND (StudentName = @StudentName OR ClassId <> @ClassId)) ORDER BY Id ASC,ClassId DESC OFFSET 20 ROW FETCH NEXT 10 rows only",
                test.Condition<Relation2>(test).Full);
        }
    }
}
