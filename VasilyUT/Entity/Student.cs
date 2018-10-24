using System;
using System.Collections.Generic;
using System.Text;
using Vasily;

namespace VasilyUT.Entity
{
    [Table("1",SqlType.MySql)]
    public class Student:IVasilyNormal
    {
        [PrimaryKey]
        public int Sid { get; set; }
    }
    [Table("1")]
    public class Student1 : IVasilyNormal
    {
        [PrimaryKey]
        public string Name { get; set; }
    }
}
