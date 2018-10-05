using MySql.Data.MySqlClient;
using Vasily;
using Xunit;

namespace VasilyUT
{

    public class UnitTest_VasilyConnectors
    {
        [Fact(DisplayName = "读-初始化器测试")]
        public void TestRead()
        {
            Connector.Add<MySqlConnection>(
                "unit", 
                "database=Read", 
                "database=Write"
                );

            DbCreator creator = Connector.ReadInitor("unit");

            Assert.NotNull(creator);
            Assert.Equal("database=Read", creator().ConnectionString);
        }
        [Fact(DisplayName = "写-初始化器测试")]
        public void TestWrite()
        {
            Connector.Add<MySqlConnection>(
                "unit",
                "database=Read",
                "database=Write"
                );

            DbCreator creator = Connector.WriteInitor("unit");

            Assert.NotNull(creator);
            Assert.Equal("database=Write", creator().ConnectionString);
        }

        [Fact(DisplayName = "读写-初始化器测试")]
        public void TestReadAndWrite()
        {
            Connector.Add<MySqlConnection>(
                "unit",
                "database=Test"
                );

            var creator = Connector.Initor("unit");
            Assert.Equal("database=Test", creator.Read().ConnectionString);
            Assert.Equal("database=Test", creator.Write().ConnectionString);
        }
    }
}
