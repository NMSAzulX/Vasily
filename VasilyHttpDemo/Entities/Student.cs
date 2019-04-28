using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vasily;

namespace VasilyHttpDemo.Entities
{
    [Table("student",SqlType.MySql)]
    public class Student:IVasilyNormal
    {
        [PrimaryKey]
        public int id { get; set; }
        public string name;
        public bool sex { get; set; }
        [Relation(typeof(ShcoolClass))]
        public int class_id;
    }
}
