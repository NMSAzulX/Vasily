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
            return PageResult(
                SqlHandler.Query(SqlCondition > "id", new { id = 142350 }), 
                SqlCondition > "id", new { id = 140635 }
                );
        }
    }
}