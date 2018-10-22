using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vasily;

namespace VasilyHttpDemo
{
    [Table("td_tmp_article_msg",SqlType.MySql)]
    public class TestEntity:IVasilyNormal
    {
        [PrimaryKey]
        public int id { get; set; }
    }
}
