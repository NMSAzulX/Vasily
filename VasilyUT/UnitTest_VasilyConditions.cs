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

        [Fact(DisplayName = "条件模糊查询测试1")]
        public void TestLikeCondition()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            SqlCondition<Relation2> condition = new SqlCondition<Relation2>();
            Assert.Equal("(StudentId > @StudentId AND StudentId LIKE @StudentId)", (condition > "StudentId" & condition  % "StudentId").ToString());
        }
        [Fact(DisplayName = "条件模糊查询测试2")]
        public void TestLikeCondition2()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            SqlCondition<Relation2> condition = new SqlCondition<Relation2>();
            Assert.Equal("((StudentId LIKE @StudentId AND StudentId > @StudentId) AND StudentId LIKE @StudentId)", ("StudentId" % condition & condition > "StudentId" & condition % "StudentId").ToString());
        }
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

        [Fact(DisplayName = "条件拼接+分页测试1")]
        public void TestPageCondition1()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            SqlCondition<Relation2> c = new SqlCondition<Relation2>();

            var cp = new { StudentId = 1, ClassId = 2, ClassName = "abc" }.
                Condition(((c > "StudentId" | c == "ClassId") & c != "ClassName")^(2,10));
            Assert.Equal(

                "((StudentId > @StudentId OR ClassId = @ClassId) AND ClassName <> @ClassName) OFFSET 10 ROW FETCH NEXT 10 rows only",

                cp.Full

                );
        }
        [Fact(DisplayName = "条件拼接+分页测试2")]
        public void TestPageCondition2()
        {
            SqlPackage<Relation> package = new SqlPackage<Relation>();
            SqlCondition<Relation> c = new SqlCondition<Relation>();

            var cp = 
                new { StudentId = 1, ClassId = 2, ClassName = "abc" }.
                Condition(c > "StudentId" | c == "ClassId" & c != "Id" ^ (2, 10));


            Assert.Equal(

                "(StudentId > @StudentId OR (ClassId = @ClassId AND Id <> @Id)) LIMIT 10,10",

                cp.Full

                );
        }
        [Fact(DisplayName = "条件拼接+排序测试1")]
        public void TestOrderCondition2()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            SqlCondition<Relation2> c = new SqlCondition<Relation2>();
            Assert.Equal(

                "((StudentId > @StudentId OR ClassId = @ClassId) AND ClassName <> @ClassName) ORDER BY StudentId ASC",

                //----------------------------条件----------------排序链接--升序------------
                //                                                    ↓   ↓
                ((c > "StudentId" | c == "ClassId") & c != "ClassName" ^ c + "StudentId").ToString()

                );
        }
        [Fact(DisplayName = "条件拼接+排序测试2")]
        public void TestPageCondition3()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            SqlCondition<Relation2> c = new SqlCondition<Relation2>();
            Assert.Equal(

                "((StudentId > @StudentId OR ClassId = @ClassId) AND ClassName <> @ClassName) ORDER BY StudentId ASC,ClassId DESC",
                //升序-----------降序-----排序链接-----------------条件----------------------------
                //↓             ↓          ↓
                (c + "StudentId" - "ClassId" ^(c > "StudentId" | c == "ClassId") & c != "ClassName").ToString()

                );
        }
    }
}
