using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VasilyHttpDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestVasilyController : VasilyController<TestEntity>
    {
        public TestVasilyController():base("key"){}
        [HttpGet]
        public ReturnPageResult GetCount()
        {
            //按照id降序排列，取第10页的前10条
            return GetsPageResult(c.Condition(c - "id" ^(10,10)) );
        }
    }
}