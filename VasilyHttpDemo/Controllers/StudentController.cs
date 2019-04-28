using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VasilyHttpDemo.Entities;

namespace VasilyHttpDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : VPReadAndWriteController<Student>
    {
        public StudentController() : base("School") { }

        [HttpGet]
        public ReturnResult GetAll()
        {
            return Result(driver.GetAll());
        }
        [HttpGet("test")]
        public ReturnResult Test()
        {
            return Result(1);
        }
        [HttpGet("test2")]
        public int Test2()
        {
            return 1;
        }
    }
}