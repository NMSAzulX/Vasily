﻿using System;
using System.Collections.Generic;
using System.Text;
using Vasily;
using Vasily.Core;
using Vasily.Engine;
using Vasily.VP;
using VasilyUT.Entity;
using Xunit;

namespace VasilyUT
{
    public class UnitTest_VasilySyntaxTree
    {

        [Fact(DisplayName = "语法树-优先级解析1")]
        public void TestNormalScript()
        {
            SqlMaker<Relation2> package = new SqlMaker<Relation2>();
            VasilyProtocal<Relation2> vp = "c<=Id&c==StudentName|c!= ClassId^c+Id- ClassId^(3,10)";
            Assert.Equal("(([Id] <= @Id AND [StudentName] = @StudentName) OR [ClassId] <> @ClassId) ORDER BY [Id] ASC,[ClassId] DESC OFFSET 20 ROW FETCH NEXT 10 rows only",
                vp.Full);
        }

        [Fact(DisplayName = "语法树-优先级解析2")]
        public void TestNormalScript1()
        {
            SqlMaker<Relation2> package = new SqlMaker<Relation2>();
            VasilyProtocal<Relation2> vp = "c<=Id&(c==StudentName|c!= ClassId)^c+Id-ClassId^(3,10)";
            Assert.Equal("([Id] <= @Id AND ([StudentName] = @StudentName OR [ClassId] <> @ClassId)) ORDER BY [Id] ASC,[ClassId] DESC OFFSET 20 ROW FETCH NEXT 10 rows only",
               vp.Full);
        }
        [Fact(DisplayName = "语法树-乱序解析1")]
        public void TestNormalScript2()
        {
            SqlMaker<Relation2> package = new SqlMaker<Relation2>();
            VasilyProtocal<Relation2> vp = "c+Id-ClassId^(3,10) ^c<=Id&(c==StudentName|c!= ClassId)";
            Assert.Equal("([Id] <= @Id AND ([StudentName] = @StudentName OR [ClassId] <> @ClassId)) ORDER BY [Id] ASC,[ClassId] DESC OFFSET 20 ROW FETCH NEXT 10 rows only",
                vp.Full);
        }
        [Fact(DisplayName = "语法树-乱序解析2")]
        public void TestNormalScript3()
        {
            SqlMaker<Relation2> package = new SqlMaker<Relation2>();
            VasilyProtocal<Relation2> vp = "c+Id-ClassId ^ c<=Id&(c==StudentName|c!= ClassId)^(3,10)";
            Assert.Equal("([Id] <= @Id AND ([StudentName] = @StudentName OR [ClassId] <> @ClassId)) ORDER BY [Id] ASC,[ClassId] DESC OFFSET 20 ROW FETCH NEXT 10 rows only",
               vp.Full);
        }

        [Fact(DisplayName = "语法树-模糊查询解析1")]
        public void TestLikeScript1()
        {
            SqlMaker<Relation2> package = new SqlMaker<Relation2>();
            VasilyProtocal<Relation2> vp = "c+Id-ClassId ^c<=Id&(c==StudentName|c!= ClassId)^(3,10) & c%StudentName";
            Assert.Equal("(([Id] <= @Id AND ([StudentName] = @StudentName OR [ClassId] <> @ClassId)) AND [StudentName] LIKE @StudentName) ORDER BY [Id] ASC,[ClassId] DESC OFFSET 20 ROW FETCH NEXT 10 rows only",
                vp.Full);
        }
        [Fact(DisplayName = "语法树-模糊查询解析2")]
        public void TestLikeScript2()
        {
            SqlMaker<Relation2> package = new SqlMaker<Relation2>();
            VasilyProtocal<Relation2> vp = "c % ClassId ^ c+Id-ClassId & c<=Id&(c==StudentName|c!= ClassId)^(3,10) & c%StudentName";
            Assert.Equal("((([ClassId] LIKE @ClassId AND [Id] <= @Id) AND ([StudentName] = @StudentName OR [ClassId] <> @ClassId)) AND [StudentName] LIKE @StudentName) ORDER BY [Id] ASC,[ClassId] DESC OFFSET 20 ROW FETCH NEXT 10 rows only",
                vp.Full);
        }
        [Fact(DisplayName = "语法树-模糊查询解析3")]
        public void TestLikeScript3()
        {
            SqlMaker<Relation2> package = new SqlMaker<Relation2>();
            VasilyProtocal<Relation2> vp = "c%StudentName ^ c+Id-ClassId  & c%ClassId ";
            
            Assert.Equal("([StudentName] LIKE @StudentName AND [ClassId] LIKE @ClassId) ORDER BY [Id] ASC,[ClassId] DESC",
                vp.Full);
        }
    }
}
