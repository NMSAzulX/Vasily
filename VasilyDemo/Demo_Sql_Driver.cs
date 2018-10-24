using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using VasilyDemo.Entities;

namespace VasilyDemo
{
    public class Demo_Sql_Driver
    {
        public static void Start()
        {
            VasilyRunner.Run();
            Connector.Add<SqlConnection>("key","链接字符串");
            Connector.Add<SqlConnection>("key-wr", "读-链接字符串","写-链接字符串");
            Connector.AddRead<SqlConnection>("read1", "读-链接字符串");
            Connector.AddWrite<SqlConnection>("write2", "写-链接字符串");

            //创建driver的三种方式
            //wrapper = wrapper1 = wrapper2
           

            DapperWrapper<One> wrapper = new DapperWrapper<One>("key");
            DapperWrapper<One> wrapper1 = "key";
            var wrapper2 = DapperWrapper<One>.UseKey("key");

            //wrapper4 = wrapper5 = wrapper6 = wrapper7
            DapperWrapper<One> wrapper4 = new DapperWrapper<One>("read1", "write2");
            DapperWrapper<One> wrapper5 = "key-wr | write2          ";
            var wrapper6 = DapperWrapper<One>.UseKey("key-wr", "write2");
            var wrapper7 = DapperWrapper<One>.UseKey("key-wr");



            One one = new One();
            One one1 = new One();
            One one2 = new One();


            //两种调用方式

            //指定操作方式,RequestType赋值一次即可。
            //RequestType默认为 Complete;
            wrapper.RequestType = VasilyRequestType.Complete;
            wrapper.GetAll();

            //使用属性调用
            wrapper.Complete.GetAll();

            //获取所有元素
            wrapper.GetAll();
            wrapper.GetByPrimary(one);
            wrapper.GetsIn(1, 2, 3, 4);
            wrapper.GetIn(1);
            wrapper.IsRepeat(one);
            wrapper.NoRepeateAdd(one);
            wrapper.GetNoRepeateId<int>(one);
            wrapper.GetRepeates(one);
            wrapper.ModifyByPrimary(one,one1,one2);
            wrapper.Add(one, one1, one2);
            wrapper.SingleDeleteByPrimary(1);
            wrapper.EntitiesDeleteByPrimary(one, one1, one2);

            //SafeInsert = NoRepeateInsert + GetNoRepeateId
            wrapper.SafeAdd(one);


        }
    }
}
