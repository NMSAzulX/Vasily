using Vasily;
using VasilyUT.Entity;
using Xunit;

namespace VasilyUT
{
    public class UnitTest_VasilyCache
    {
        [Fact(DisplayName = "缓存-SQL生成器Model测试")]
        public void TestSelectRelation32()
        {
            SqlPackage<Relation> package = new SqlPackage<Relation>();
            Assert.Equal("Id", SqlModel<Relation>.PrimaryKey);
            Assert.Equal('`', SqlModel<Relation>.Left);
            Assert.Equal('`', SqlModel<Relation>.Right);
            Assert.Equal("关系映射表", SqlModel<Relation>.TableName);
            Assert.NotNull(SqlModel<Relation>.ColumnMapping);
        }
    }
}
