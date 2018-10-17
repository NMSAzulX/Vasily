using System;
using System.Collections.Generic;
using System.Text;
using Vasily;

namespace VasilyUT.Entity
{
    [Table("AB")]
    public class Class:IVasilyNormal
    {
        [PrimaryKey]
        public int Cid;
        public string Other;
    }
    [Table("AB")]
    public class Class1 : IVasilyNormal
    {
        [PrimaryKey]
        public string Name;
    }
    
}
