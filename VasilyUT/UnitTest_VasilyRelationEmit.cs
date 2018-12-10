using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Vasily;
using Vasily.Core;
using VasilyUT.Entity;
using Xunit;

namespace VasilyUT
{
    public class UnitTest_VasilyRelationEmit
    {

        [Fact(DisplayName = "属性值类型Setter/Getter测试")]
        public void TestValueProperty()
        {
            NormalAnalysis<Relation2> package = new NormalAnalysis<Relation2>();
            NormalAnalysis<Student> package1 = new NormalAnalysis<Student>();
            Student student = new Student();
            SqlEntity<Student>.SetPrimary(student, 1);
            Assert.Equal(1, (int)RelationSql<Student, Relation2, Student1, Class, Class1>.Getters[0](student));

        }
        [Fact(DisplayName = "属性引用类型Setter/Getter测试")]
        public void TestRefProperty()
        {
            NormalAnalysis<Relation2> package = new NormalAnalysis<Relation2>();
            NormalAnalysis<Student1> package1 = new NormalAnalysis<Student1>();
            Student1 student = new Student1();
            SqlEntity<Student1>.SetPrimary(student, "abc");
            Assert.Equal("abc", (string)RelationSql<Student1, Relation2, Student, Class, Class1>.Getters[0](student));

        }

        [Fact(DisplayName = "字段值类型Setter/Getter测试")]
        public void TestValueField()
        {
            NormalAnalysis<Relation2> package = new NormalAnalysis<Relation2>();
            NormalAnalysis<Class> package1 = new NormalAnalysis<Class>();
            Class myClass = new Class();
            SqlEntity<Class>.SetPrimary(myClass,1);
           Assert.Equal(1, (int)RelationSql<Class, Relation2, Student1, Student, Class1>.Getters[0](myClass));

        }
        [Fact(DisplayName = "字段引用类型Setter/Getter测试")]
        public void TestRefField()
        {
            NormalAnalysis<Relation2> package = new NormalAnalysis<Relation2>();
            NormalAnalysis<Class1> package1 = new NormalAnalysis<Class1>();
            Class1 myClass = new Class1();
            SqlEntity<Class1>.SetPrimary(myClass, "abc");
            Assert.Equal("abc", (string)RelationSql<Class1, Relation2, Student1, Class, Student>.Getters[0](myClass));
        }
    }
}
