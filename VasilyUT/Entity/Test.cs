using System;
using System.Collections.Generic;
using System.Text;
using Vasily;

namespace VasilyUT.Entity
{
    [Table("Table")]
    
    public class Test:IVasilyNormal
    {
        [PrimaryKey]
        public int Id;

        [NoRepeate]
        public string NoRepeate;

        [Ignore]
        public string Ignore1;

        [Ignore]
        public string Ignore2;
    }
}
