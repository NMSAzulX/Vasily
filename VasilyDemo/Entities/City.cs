using System;
using Vasily;

namespace VasilyDemo.Entities
{
    [Table("tb_city", SqlType.MySql)]
    public class City:IVasilyNormal
    {
        [PrimaryKey]
        [Relation(typeof(City))]
        public int id { get; set; }
        public string name { get; set; }


        //让parent_id指向自身
        [Relation(typeof(City_Anyname), "id")]
        public int parent_id { get; set; }
    }

    public class City_Anyname { }

}
