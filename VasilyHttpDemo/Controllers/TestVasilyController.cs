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
            //return BoolPageResult(true, SqlCondition > "id",new { id= 140635 });
            return GetsResult(
                new { id = 142350 }.Condition(c>"id"), //按条件查询集合
                new { id = 140635 }.Condition(c>"id")  //按条件查询总数
                );
        }
    }
}