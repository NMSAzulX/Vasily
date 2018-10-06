using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Vasily;
using VasilyUT.Entity;
using Xunit;

namespace VasilyUT
{
    public class UnitTest_VasilyRelationEmit
    {

        [Fact(DisplayName = "属性值类型Getter测试")]
        public void TestValueProperty()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            Student student = new Student();
            student.Sid = 1;
            Assert.Equal(1, (int)RelationSql<Student, Relation2, Student1, Class, Class1>.Getters[0](student));

        }
        [Fact(DisplayName = "属性引用类型Getter测试")]
        public void TestRefProperty()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            Student1 student = new Student1();
            student.Name = "abc";
            Assert.Equal("abc", (string)RelationSql<Student1, Relation2, Student, Class, Class1>.Getters[0](student));

        }

        [Fact(DisplayName = "字段值类型Getter测试")]
        public void TestValueField()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            Class myClass = new Class();
            myClass.Cid = 1;
            Assert.Equal(1, (int)RelationSql<Class, Relation2, Student1, Student, Class1>.Getters[0](myClass));

        }
        [Fact(DisplayName = "字段引用类型Getter测试")]
        public void TestRefField()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            Class1 myClass = new Class1();
            myClass.Name = "abc";
            Assert.Equal("abc", (string)RelationSql<Class1, Relation2, Student1, Class, Student>.Getters[0](myClass));

        }
    }
}
