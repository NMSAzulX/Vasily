using System;
using Vasily;

namespace VasilyDemo.Entities
{
    [Table("table_one",SqlType.MsSql)]
    public class One:IVasilyNormal
    {
        [PrimaryKey]
        public int oid { get; set; }

        public string name { get; set; }

        [UpdateIgnore]
        public int create_time { get; set; }

        public int update_time { get; set; }

        public int age { get; set; }

        [NoRepeate]
        public long student_id { get; set; }

    }
}
