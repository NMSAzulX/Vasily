using System;
using Vasily;

namespace VasilyDemo.Entities
{
    [Table("tb_class", SqlType.MySql)]
    public class SchoolClass : IVasilyNormal
    {
        [PrimaryKey]
        public int cid { get; set; }


        public string name { get; set; }


        [Column("create time")]
        public int create_time { get; set; }
    }
}
