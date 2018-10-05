using Vasily;
using Vasily.Core;
using Vasily.Sql;
using Xunit;

namespace VasilyUT
{

    public class UnitTest_VasilyStaticEmit
    {
        [Fact(DisplayName = "无泛型的Operator")]
        public void TesZeroStatic()
        {
            var operator1 = new GsOperator(typeof(Sql<>),typeof(int));
            operator1.Set("SelectAll", "MsSqlOne");


            var operator2 = new GsOperator(typeof(Sql<>),typeof(string));
            operator2.Set("Insert", "MySqlOne");


            Assert.Equal("MsSqlOne", Sql<int>.SelectAll);
            Assert.Equal("MySqlOne", Sql<string>.Insert);
        }
        [Fact(DisplayName = "一个泛型的Operator")]
        public void TestOneStatic()
        {
            var operator1 = new GsOperator<Sql<GenericType>>(typeof(int));
            operator1.Set("SelectAll", "MsSqlOne");

            
            var operator2 = new GsOperator<Sql<GenericType>>(typeof(string));
            operator2.Set("SelectAll", "MySqlOne1");


            Assert.Equal("MsSqlOne", Sql<int>.SelectAll);
            Assert.Equal("MySqlOne1", Sql<string>.SelectAll);
        }

        [Fact(DisplayName = "两个泛型的Operator")]
        public void TestTwoStatic()
        {
            var operator1 = new GsOperator<Sql<GenericType>,int>();
            operator1.Set("SelectAll", "MsSqlOne");

           
            var operator2 = new GsOperator<Sql<GenericType>, string>();
            operator2.Set("Insert", "MySqlOne");


            Assert.Equal("MsSqlOne", Sql<int>.SelectAll);
            Assert.Equal("MySqlOne", Sql<string>.Insert);
        }
    }
}
