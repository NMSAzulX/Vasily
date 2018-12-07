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

        [Fact(DisplayName = "属性值类型Setter/Getter测试")]
        public void TestValueProperty()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            SqlPackage<Student> package1 = new SqlPackage<Student>();
            Student student = new Student();
            Sql<Student>.SetPrimary(student, 1);
            Assert.Equal(1, (int)RelationSql<Student, Relation2, Student1, Class, Class1>.Getters[0](student));

        }
        [Fact(DisplayName = "属性引用类型Setter/Getter测试")]
        public void TestRefProperty()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            SqlPackage<Student1> package1 = new SqlPackage<Student1>();
            Student1 student = new Student1();
            Sql<Student1>.SetPrimary(student, "abc");
            Assert.Equal("abc", (string)RelationSql<Student1, Relation2, Student, Class, Class1>.Getters[0](student));

        }

        [Fact(DisplayName = "字段值类型Setter/Getter测试")]
        public void TestValueField()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            SqlPackage<Class> package1 = new SqlPackage<Class>();
            Class myClass = new Class();
            Sql<Class>.SetPrimary(myClass,1);
           Assert.Equal(1, (int)RelationSql<Class, Relation2, Student1, Student, Class1>.Getters[0](myClass));

        }
        [Fact(DisplayName = "字段引用类型Setter/Getter测试")]
        public void TestRefField()
        {
            SqlPackage<Relation2> package = new SqlPackage<Relation2>();
            SqlPackage<Class1> package1 = new SqlPackage<Class1>();
            Class1 myClass = new Class1();
            Sql<Class1>.SetPrimary(myClass, "abc");
            Assert.Equal("abc", (string)RelationSql<Class1, Relation2, Student1, Class, Student>.Getters[0](myClass));
        }
    }
}
