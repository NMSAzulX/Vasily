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
            Assert.Equal("Id", MakerModel<Relation>.PrimaryKey);
            Assert.Equal('`', MakerModel<Relation>.Left);
            Assert.Equal('`', MakerModel<Relation>.Right);
            Assert.Equal("关系映射表", MakerModel<Relation>.TableName);
            Assert.NotNull(MakerModel<Relation>.StringMapping);
            Assert.NotNull(MakerModel<Relation>.ColumnMapping);
        }
    }
}
