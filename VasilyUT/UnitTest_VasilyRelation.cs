using System;
using Vasily;
using Vasily.Core;
using Vasily.Engine;
using VasilyUT.Entity;
using Xunit;

namespace VasilyUT
{
    public class UnitTest_VasilyRelation
    {
        [Fact(DisplayName = "A32-关系数组")]
        public void TestTableArray()
        {
            //SqlMaker<Relation> package = new SqlMaker<Relation>();
            Assert.Equal("StudentId,Id", string.Join(',', RelationSql<Student, Relation, Relation>.TableConditions));
        }

        [Fact(DisplayName = "A32-总数查询")]
        public void TestTableCount()
        {
            //SqlMaker<Relation> package = new SqlMaker<Relation>();
            Assert.Equal("SELECT COUNT(*) FROM `1` AS `V_1_TA` INNER JOIN `关系映射表` AS `V_关系映射表_TB` ON `V_1_TA`.`Sid`=`V_关系映射表_TB`.`StudentId` AND `V_关系映射表_TB`.`Id`=@Id",  RelationSql<Student, Relation, Relation>.CountFromTable);
        }
        [Fact(DisplayName = "A32-总数查询")]
        public void TestSourceCount()
        {
            //SqlMaker<Relation> package = new SqlMaker<Relation>();
            Assert.Equal("SELECT COUNT(*) FROM `关系映射表` AS `V_关系映射表_TA` INNER JOIN `关系映射表` AS `V_关系映射表_TB` ON `V_关系映射表_TA`.`Id`=`V_关系映射表_TB`.`Id` AND `V_关系映射表_TB`.`StudentId`=@Sid", RelationSql<Relation, Relation, Student>.CountFromSource);
        }
        [Fact(DisplayName = "A32-关系数组")]
        public void TestSourceArray()
        {
            //SqlMaker<Relation> package = new SqlMaker<Relation>();
            Assert.Equal("Sid,Id", string.Join(',', RelationSql<Student, Relation, Relation>.SourceConditions));
        }
        [Fact(DisplayName = "A32-查询-关系表生成测试")]
        public void TestSelectRelation32()
        {
           

            Assert.Equal("SELECT * FROM `1` AS `V_1_TA` INNER JOIN `关系映射表` AS `V_关系映射表_TB` ON `V_1_TA`.`Sid`=`V_关系映射表_TB`.`StudentId` AND `V_关系映射表_TB`.`Id`=@Id", RelationSql<Student,Relation,Relation>.GetFromTable);
            Assert.Equal("SELECT * FROM `AB` AS `V_AB_TA` INNER JOIN `关系映射表` AS `V_关系映射表_TB` ON `V_AB_TA`.`Cid`=`V_关系映射表_TB`.`ClassId` AND `V_关系映射表_TB`.`Id`=@Id", RelationSql<Class, Relation, Relation>.GetFromTable);
            Assert.Equal("SELECT * FROM `AB` AS `V_AB_TA` INNER JOIN `关系映射表` AS `V_关系映射表_TB` ON `V_AB_TA`.`Cid`=`V_关系映射表_TB`.`ClassId` AND `V_关系映射表_TB`.`StudentId`=@StudentId", RelationSql<Class, Relation, Student>.GetFromTable);
            Assert.Equal("SELECT * FROM `1` AS `V_1_TA` INNER JOIN `关系映射表` AS `V_关系映射表_TB` ON `V_1_TA`.`Sid`=`V_关系映射表_TB`.`StudentId` AND `V_关系映射表_TB`.`ClassId`=@ClassId", RelationSql<Student, Relation, Class>.GetFromTable);


            Assert.Equal("SELECT * FROM `1` AS `V_1_TA` INNER JOIN `关系映射表` AS `V_关系映射表_TB` ON `V_1_TA`.`Sid`=`V_关系映射表_TB`.`StudentId` AND `V_关系映射表_TB`.`ClassId`=@Cid", RelationSql<Student, Relation, Class>.GetFromSource);
            Assert.Equal("SELECT * FROM `关系映射表` AS `V_关系映射表_TA` INNER JOIN `关系映射表` AS `V_关系映射表_TB` ON `V_关系映射表_TA`.`Id`=`V_关系映射表_TB`.`Id` AND `V_关系映射表_TB`.`StudentId`=@Sid", RelationSql<Relation, Relation, Student>.GetFromSource);
            Assert.Equal("SELECT * FROM `AB` AS `V_AB_TA` INNER JOIN `关系映射表` AS `V_关系映射表_TB` ON `V_AB_TA`.`Cid`=`V_关系映射表_TB`.`ClassId` AND `V_关系映射表_TB`.`StudentId`=@Sid", RelationSql<Class, Relation, Student>.GetFromSource);
            Assert.Equal("SELECT * FROM `1` AS `V_1_TA` INNER JOIN `关系映射表` AS `V_关系映射表_TB` ON `V_1_TA`.`Sid`=`V_关系映射表_TB`.`StudentId` AND `V_关系映射表_TB`.`Id`=@Id", RelationSql<Student, Relation, Relation>.GetFromSource);
        }
        [Fact(DisplayName = "A32-更新-关系表生成测试")]
        public void TestUpdateRelation32()
        {
            SqlMaker<Relation> package = new SqlMaker<Relation>();

            Assert.Equal("UPDATE `关系映射表` SET `StudentId`=@StudentId WHERE `Id`=@Id", RelationSql<Student, Relation, Relation>.ModifyFromTable);
            //Assert.Equal("UPDATE `关系映射表` SET `Id`=@Id WHERE `ClassId`=@ClassId", RelationSql<Relation, Relation, Class>.ModifyFromTable);

            Assert.Equal("UPDATE `关系映射表` SET `StudentId`=@Sid WHERE `ClassId`=@Cid", RelationSql<Student, Relation, Class>.ModifyFromSource);
            Assert.Equal("UPDATE `关系映射表` SET `ClassId`=@Cid WHERE `StudentId`=@Sid", RelationSql<Class, Relation, Student>.ModifyFromSource);
        }

        [Fact(DisplayName = "A32-前置删除-关系表生成测试")]
        public void TestDeletePreRelation32()
        {
            SqlMaker<Relation> package = new SqlMaker<Relation>();

            Assert.Equal("DELETE FROM `关系映射表` WHERE `StudentId`=@StudentId", RelationSql<Student, Relation, Relation>.DeletePreFromTable);
            Assert.Equal("DELETE FROM `关系映射表` WHERE `Id`=@Id", RelationSql<Relation, Relation, Class>.DeletePreFromTable);

            Assert.Equal("DELETE FROM `关系映射表` WHERE `StudentId`=@Sid", RelationSql<Student, Relation, Class>.DeletePreFromSource);
            Assert.Equal("DELETE FROM `关系映射表` WHERE `ClassId`=@Cid", RelationSql<Class, Relation, Student>.DeletePreFromSource);
        }
        [Fact(DisplayName = "A32-后置删除-关系表生成测试")]
        public void TestDeleteAftRelation32()
        {
            //SqlMaker<Relation> package = new SqlMaker<Relation>();

            Assert.Equal("DELETE FROM `关系映射表` WHERE `Id`=@Id", RelationSql<Student, Relation, Relation>.DeleteAftFromTable);
            Assert.Equal("DELETE FROM `关系映射表` WHERE `ClassId`=@ClassId", RelationSql<Relation, Relation, Class>.DeleteAftFromTable);

            Assert.Equal("DELETE FROM `关系映射表` WHERE `ClassId`=@Cid", RelationSql<Student, Relation, Class>.DeleteAftFromSource);
            Assert.Equal("DELETE FROM `关系映射表` WHERE `StudentId`=@Sid", RelationSql<Class, Relation, Student>.DeleteAftFromSource);
        }
        [Fact(DisplayName = "A32-插入-关系表生成测试")]
        public void TestInsertRelation32()
        { 
            Assert.Equal("INSERT INTO `关系映射表` (`StudentId`)VALUES(@StudentId)", RelationSql<Student, Relation, Relation>.AddFromTable);
            //Assert.Equal("INSERT INTO `关系映射表` (`ClassId`)VALUES(@ClassId)", RelationSql<Relation, Relation, Class>.AddFromTable);

            //Assert.Equal("INSERT INTO `关系映射表` (`StudentId`,`ClassId`)VALUES(@Sid,@Cid)", RelationSql<Student, Relation, Class>.AddFromSource);
            //Assert.Equal("INSERT INTO `关系映射表` (`ClassId`,`StudentId`)VALUES(@Cid,@Sid)", RelationSql<Class, Relation, Student>.AddFromSource);
        }
        [Fact(DisplayName = "A33-查询-关系表生成测试")]
        public void TestSelectRelation33()
        {
            SqlMaker<Relation> package = new SqlMaker<Relation>();

            Assert.Equal("SELECT * FROM `1` AS `V_1_TA` INNER JOIN `关系映射表` AS `V_关系映射表_TB` ON `V_1_TA`.`Sid`=`V_关系映射表_TB`.`StudentId` AND `V_关系映射表_TB`.`Id`=@Id AND `V_关系映射表_TB`.`ClassId`=@ClassId", RelationSql<Student, Relation, Relation,Class>.GetFromTable);
            Assert.Equal("SELECT * FROM `1` AS `V_1_TA` INNER JOIN `关系映射表` AS `V_关系映射表_TB` ON `V_1_TA`.`Sid`=`V_关系映射表_TB`.`StudentId` AND `V_关系映射表_TB`.`ClassId`=@Cid AND `V_关系映射表_TB`.`Id`=@Id", RelationSql<Student, Relation, Class, Relation>.GetFromSource);

        }
    }
}
