using System;
using System.Collections.Generic;
using System.Text;
using Vasily;
using VasilyUT.Entity;
using Xunit;

namespace VasilyUT
{
    public class UnitTest_VasilyConditions
    {
        [Fact(DisplayName = "条件拼接测试1")]
        public void TestCondition()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            SqlCondition<Relation2> condition = new SqlCondition<Relation2>();
            Assert.Equal("StudentId > @StudentId", (condition > "StudentId").ToString());
         }
        [Fact(DisplayName = "条件拼接测试2")]
        public void TestCondition2()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            SqlCondition<Relation2> condition = new SqlCondition<Relation2>();
            Assert.Equal("(StudentId > @StudentId OR ClassId <> @ClassId)", (condition > "StudentId" | condition != "ClassId").ToString());
        }
        [Fact(DisplayName = "条件拼接测试3")]
        public void TestCondition3()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            SqlCondition<Relation2> c = new SqlCondition<Relation2>();
            Assert.Equal(

                "((StudentId > @StudentId OR ClassId = @ClassId) AND ClassName <> @ClassName)", 

                ((c > "StudentId" | c == "ClassId") & c != "ClassName").ToString()
                       
                );
        }
    }
}
