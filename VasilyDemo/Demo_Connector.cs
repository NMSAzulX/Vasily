using System.Data;
using System.Data.SqlClient;
using Vasily;

namespace VasilyDemo
{
    class Demo_Connector
    {
        public void Demo()
        {
            //添加一个SqlServer驱动的读写字符串
            //key为"key:sql"
            //连接字符串为"连接字符串"
            Connector.Add<SqlConnection>("key:sql1", "连接字符串");
            Connector.Add<SqlConnection>("key:sql2", "写-连接字符串","读-连接字符串");
            Connector.Add<SqlConnection,SqlConnection>("key:sql2", "写-连接字符串", "读-连接字符串");

            //获取"key:sql"的连接初始化器
            var creator = Connector.Initor("key:sql1");
            var reader = Connector.ReadInitor("key:sql1");
            var writter = Connector.WriteInitor("key:sql2");

            //获取连接委托
            DbCreator read = creator.Read;
            DbCreator write = creator.Write;

            //创建连接
            IDbConnection r_connection = read();
            IDbConnection w_connection = write();

        }
    }
}
