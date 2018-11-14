using System;
using Vasily;

namespace VasilyUT.Entity
{
    [Table("关系映射表", SqlType.MySql)]
    public class Relation : IVasilyNormal
    {
        [PrimaryKey]
        [Relation(typeof(Relation), "Id")]
        public int Id;

        [Relation(typeof(Student), "Sid")]
        public int StudentId;

        [Relation(typeof(Class))]
        public int ClassId;
    }

    [Table("关系映射表2", SqlType.MsSql)]
    public class Relation2 : IVasilyNormal
    {
        [PrimaryKey]
        [Relation(typeof(Relation2), "Id")]
        public int Id;

        [Relation(typeof(Student), "Sid")]
        public int StudentId;

        [Relation(typeof(Student1), "Name")]
        public int StudentName;

        [Relation(typeof(Class))]
        public int ClassId;

        [Relation(typeof(Class1), "Name")]
        public int ClassName;
    }
}
