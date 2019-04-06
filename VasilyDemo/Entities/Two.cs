using System;
using Vasily;

namespace VasilyDemo.Entities
{
    [Table("table_relation", SqlType.MsSql)]
    public class Two
    {
        [PrimaryKey]
        [Relation(typeof(Two))]
        public int rid { get; set; }

        [Relation(typeof(One))]
        public int one_id { get; set; }

        [Relation(typeof(Three))]
        public int three_id { get; set; }

        [Relation(typeof(Two_Parent), "rid")]
        public int parent_id { get; set; }

    }

    public class Three
    {
        [PrimaryKey]
        public int tid { get; set; }
    }

    public class Two_Parent { }
}
