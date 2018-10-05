using Vasily;

namespace VasilyUT.Entity
{
    [Table("关系映射表", SqlType.MySql)]
    public class Relation : IVasilyRelation
    {
        [PrimaryKey]
        [Relation(typeof(Relation), "Id")]
        public int Id;

        [Relation(typeof(Student), "Sid")]
        public int StudentId;

        [Relation(typeof(Class))]
        public int ClassId;
    }
}
