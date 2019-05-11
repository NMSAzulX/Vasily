using Vasily;

namespace VasilyDemo.Entities
{
    [Table("tb_student_class", SqlType.MySql)]
    public class StudentAndClass
    {
        [PrimaryKey]
        public int rid { get; set; }


        //若不传"cid", 则默认使用Class的主键
        [Relation(typeof(SchoolClass), "cid")]
        public int class_id { get; set; }


        //student_id并没有对应学生的id,而是对应了学生表里student_id
        [Relation(typeof(Student), "student_id")]
        public int student_id { get; set; }
    }
}
