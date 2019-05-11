using System;
using Vasily;

namespace VasilyDemo.Entities
{
    [Table("tb_student", SqlType.MySql)]
    public class Student : IVasilyNormal
    {
        [PrimaryKey]
        public int oid { get; set; }


        [NoRepeate]
        public long student_id { get; set; }


        public int age { get; set; }
        public string name { get; set; }


        [Column("create time")]
        public int create_time { get; set; }
        [Column("last time")]
        public int update_time { get; set; }


        [Ignore]
        public string[] Children;

    }
}
