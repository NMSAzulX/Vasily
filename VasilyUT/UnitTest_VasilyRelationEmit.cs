﻿using Vasily;
using Vasily.Engine;
using VasilyUT.Entity;
using Xunit;

namespace VasilyUT
{
    public class UnitTest_VasilyRelationEmit
    {

        [Fact(DisplayName = "属性值类型Setter/Getter测试")]
        public void TestValueProperty()
        {
            SqlMaker<Relation2> package = new SqlMaker<Relation2>();
            SqlMaker<Student> package1 = new SqlMaker<Student>();
            Student student = new Student();
            SqlEntity<Student>.SetPrimary(student, 1);
            Assert.Equal(1, (int)RelationSql<Student, Relation2, Student1, Class, Class1>.Getters[0](student));

        }
        [Fact(DisplayName = "属性引用类型Setter/Getter测试")]
        public void TestRefProperty()
        {
            SqlMaker<Relation2> package = new SqlMaker<Relation2>();
            SqlMaker<Student1> package1 = new SqlMaker<Student1>();
            Student1 student = new Student1();
            SqlEntity<Student1>.SetPrimary(student, "abc");
            Assert.Equal("abc", (string)RelationSql<Student1, Relation2, Student, Class, Class1>.Getters[0](student));

        }

        [Fact(DisplayName = "字段值类型Setter/Getter测试")]
        public void TestValueField()
        {
            SqlMaker<Relation2> package = new SqlMaker<Relation2>();
            SqlMaker<Class> package1 = new SqlMaker<Class>();
            Class myClass = new Class();
            SqlEntity<Class>.SetPrimary(myClass,1);
           Assert.Equal(1, (int)RelationSql<Class, Relation2, Student1, Student, Class1>.Getters[0](myClass));

        }
        [Fact(DisplayName = "字段引用类型Setter/Getter测试")]
        public void TestRefField()
        {
            SqlMaker<Relation2> package = new SqlMaker<Relation2>();
            SqlMaker<Class1> package1 = new SqlMaker<Class1>();
            Class1 myClass = new Class1();
            SqlEntity<Class1>.SetPrimary(myClass, "abc");
            Assert.Equal("abc", (string)RelationSql<Class1, Relation2, Student1, Class, Student>.Getters[0](myClass));
        }
    }
}
