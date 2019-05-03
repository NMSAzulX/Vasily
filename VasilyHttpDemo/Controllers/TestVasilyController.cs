using Microsoft.AspNetCore.Mvc;
using Vasily.VP;

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
            //return GetsPageResult(c.Condition(c - "id" ^(10,10)) );

            //从前端传过来一个查询JSON格式
            /*{
                   value:{
                       id:10000
                   },
                   sql:"c>id ^c - id ^(3,10)",
                   unions:[
                       "tb_table1",
                       "tb_table2"
                   ]
            }*/

            //模拟POST

            VasilyProtocal<TestEntity> vp = new VasilyProtocal<TestEntity>();
            vp.Script = "c>id ^c - id ^(3,10)";
            vp.Instance = new TestEntity() { id = 1000 };

            UseUnion("td_teacher1", "td_teacher2");
            return GetsPageResult(vp);
        }
    }
}