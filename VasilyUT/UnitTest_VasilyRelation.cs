using System;
using Vasily;
using VasilyUT.Entity;
using Xunit;

namespace VasilyUT
{
    public class UnitTest_VasilyRelation
    {
        [Fact(DisplayName = "A32-关系数组")]
        public void TestTableArray()
        {
            SqlPackage<Relation> package = new SqlPackage<Relation>();
            Assert.Equal("StudentId,Id", string.Join(',', RelationSql<Student, Relation, Relation>.TableConditions));
        }

        [Fact(DisplayName = "A32-总数查询")]
        public void TestTableCount()
        {
            SqlPackage<Relation> package = new SqlPackage<Relation>();
            Assert.Equal("SELECT Count(*) FROM `关系映射表` WHERE `Id`=@Id",  RelationSql<Student, Relation, Relation>.CountFromTable);
        }
        [Fact(DisplayName = "A32-总数查询")]
        public void TestSourceCount()
        {
            SqlPackage<Relation> package = new SqlPackage<Relation>();
            Assert.Equal("SELECT Count(*) FROM `关系映射表` WHERE `StudentId`=@Sid", RelationSql<Relation, Relation, Student>.CountFromSource);
        }
        [Fact(DisplayName = "A32-关系数组")]
        public void TestSourceArray()
        {
            SqlPackage<Relation> package = new SqlPackage<Relation>();
            Assert.Equal("Sid,Id", string.Join(',', RelationSql<Student, Relation, Relation>.SourceConditions));
        }
        [Fact(DisplayName = "A32-查询-关系表生成测试")]
        public void TestSelectRelation32()
        {
            SqlPackage<Relation> package = new SqlPackage<Relation>();

            Assert.Equal("SELECT `StudentId` FROM `关系映射表` WHERE `Id`=@Id", RelationSql<Student,Relation,Relation>.GetFromTable);
            Assert.Equal("SELECT `ClassId` FROM `关系映射表` WHERE `Id`=@Id", RelationSql<Class, Relation, Relation>.GetFromTable);
            Assert.Equal("SELECT `ClassId` FROM `关系映射表` WHERE `StudentId`=@StudentId", RelationSql<Class, Relation, Student>.GetFromTable);
            Assert.Equal("SELECT `StudentId` FROM `关系映射表` WHERE `ClassId`=@ClassId", RelationSql<Student, Relation, Class>.GetFromTable);


            Assert.Equal("SELECT `StudentId` FROM `关系映射表` WHERE `ClassId`=@Cid", RelationSql<Student, Relation, Class>.GetFromSource);
            Assert.Equal("SELECT `ClassId` FROM `关系映射表` WHERE `StudentId`=@Sid", RelationSql<Class, Relation, Student>.GetFromSource);
            Assert.Equal("SELECT `Id` FROM `关系映射表` WHERE `StudentId`=@Sid", RelationSql<Relation, Relation, Student>.GetFromSource);
            Assert.Equal("SELECT `StudentId` FROM `关系映射表` WHERE `Id`=@Id", RelationSql<Student, Relation, Relation>.GetFromSource);
        }
        [Fact(DisplayName = "A32-更新-关系表生成测试")]
        public void TestUpdateRelation32()
        {
            SqlPackage<Relation> package = new SqlPackage<Relation>();

            Assert.Equal("UPDATE `关系映射表` SET `StudentId`=@StudentId WHERE `Id`=@Id", RelationSql<Student, Relation, Relation>.ModifyFromTable);
            Assert.Equal("UPDATE `关系映射表` SET `Id`=@Id WHERE `ClassId`=@ClassId", RelationSql<Relation, Relation, Class>.ModifyFromTable);

            Assert.Equal("UPDATE `关系映射表` SET `StudentId`=@Sid WHERE `ClassId`=@Cid", RelationSql<Student, Relation, Class>.ModifyFromSource);
            Assert.Equal("UPDATE `关系映射表` SET `ClassId`=@Cid WHERE `StudentId`=@Sid", RelationSql<Class, Relation, Student>.ModifyFromSource);
        }

        [Fact(DisplayName = "A32-前置删除-关系表生成测试")]
        public void TestDeletePreRelation32()
        {
            SqlPackage<Relation> package = new SqlPackage<Relation>();

            Assert.Equal("DELETE FROM `关系映射表` WHERE `StudentId`=@StudentId", RelationSql<Student, Relation, Relation>.DeletePreFromTable);
            Assert.Equal("DELETE FROM `关系映射表` WHERE `Id`=@Id", RelationSql<Relation, Relation, Class>.DeletePreFromTable);

            Assert.Equal("DELETE FROM `关系映射表` WHERE `StudentId`=@Sid", RelationSql<Student, Relation, Class>.DeletePreFromSource);
            Assert.Equal("DELETE FROM `关系映射表` WHERE `ClassId`=@Cid", RelationSql<Class, Relation, Student>.DeletePreFromSource);
        }
        [Fact(DisplayName = "A32-后置删除-关系表生成测试")]
        public void TestDeleteAftRelation32()
        {
            SqlPackage<Relation> package = new SqlPackage<Relation>();

            Assert.Equal("DELETE FROM `关系映射表` WHERE `Id`=@Id", RelationSql<Student, Relation, Relation>.DeleteAftFromTable);
            Assert.Equal("DELETE FROM `关系映射表` WHERE `ClassId`=@ClassId", RelationSql<Relation, Relation, Class>.DeleteAftFromTable);

            Assert.Equal("DELETE FROM `关系映射表` WHERE `ClassId`=@Cid", RelationSql<Student, Relation, Class>.DeleteAftFromSource);
            Assert.Equal("DELETE FROM `关系映射表` WHERE `StudentId`=@Sid", RelationSql<Class, Relation, Student>.DeleteAftFromSource);
        }
        [Fact(DisplayName = "A32-插入-关系表生成测试")]
        public void TestInsertRelation32()
        {
            SqlPackage<Relation> package = new SqlPackage<Relation>();

            Assert.Equal("INSERT INTO `关系映射表` (`StudentId`,`Id`)VALUES(@StudentId,@Id)", RelationSql<Student, Relation, Relation>.AddFromTable);
            Assert.Equal("INSERT INTO `关系映射表` (`Id`,`ClassId`)VALUES(@Id,@ClassId)", RelationSql<Relation, Relation, Class>.AddFromTable);

            Assert.Equal("INSERT INTO `关系映射表` (`StudentId`,`ClassId`)VALUES(@Sid,@Cid)", RelationSql<Student, Relation, Class>.AddFromSource);
            Assert.Equal("INSERT INTO `关系映射表` (`ClassId`,`StudentId`)VALUES(@Cid,@Sid)", RelationSql<Class, Relation, Student>.AddFromSource);
        }
        [Fact(DisplayName = "A33-查询-关系表生成测试")]
        public void TestSelectRelation33()
        {
            SqlPackage<Relation> package = new SqlPackage<Relation>();

            Assert.Equal("SELECT `StudentId` FROM `关系映射表` WHERE `Id`=@Id AND `ClassId`=@ClassId", RelationSql<Student, Relation, Relation,Class>.GetFromTable);
            
            Assert.Equal("SELECT `StudentId` FROM `关系映射表` WHERE `ClassId`=@Cid AND `Id`=@Id", RelationSql<Student, Relation, Class, Relation>.GetFromSource);

        }
    }
}
