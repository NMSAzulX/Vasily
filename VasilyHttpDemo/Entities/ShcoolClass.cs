using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vasily;

namespace VasilyHttpDemo.Entities
{
    [Table("class")]
    public class ShcoolClass
    {
        [PrimaryKey]
        public int id;
        public string name;
        public int count;
    }
}
