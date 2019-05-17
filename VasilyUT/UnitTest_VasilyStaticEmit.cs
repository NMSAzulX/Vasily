using System;
using Vasily;
using Vasily.Engine;
using Vasily.Engine.Utils;
using Xunit;

namespace VasilyUT
{

    public class UnitTest_VasilyStaticEmit
    {
        [Fact(DisplayName = "无泛型的Operator")]
        public void TesZeroStatic()
        {

            var operator1 = new GsOperator(typeof(SqlEntity<>),typeof(int));
            operator1.Set("SelectAll", "MsSqlOne");


            var operator2 = new GsOperator(typeof(SqlEntity<>),typeof(string));
            operator2.Set("InsertAll", "MySqlOne");


            Assert.Equal("MsSqlOne", SqlEntity<int>.SelectAll);
            Assert.Equal("MySqlOne", SqlEntity<string>.InsertAll);
        }
        [Fact(DisplayName = "一个泛型的Operator")]
        public void TestOneStatic()
        {
            var operator1 = new GsOperator<SqlEntity<GenericType>>(typeof(int));
            operator1.Set("SelectAll", "MsSqlOne");

            
            var operator2 = new GsOperator<SqlEntity<GenericType>>(typeof(string));
            operator2.Set("SelectAll", "MySqlOne1");


            Assert.Equal("MsSqlOne", SqlEntity<int>.SelectAll);
            Assert.Equal("MySqlOne1", SqlEntity<string>.SelectAll);
        }

        [Fact(DisplayName = "两个泛型的Operator")]
        public void TestTwoStatic()
        {
            var operator1 = new GsOperator<SqlEntity<GenericType>,int>();
            operator1.Set("SelectAll", "MsSqlOne");

           
            var operator2 = new GsOperator<SqlEntity<GenericType>, string>();
            operator2.Set("InsertAll", "MySqlOne");


            Assert.Equal("MsSqlOne", SqlEntity<int>.SelectAll);
            Assert.Equal("MySqlOne", SqlEntity<string>.InsertAll);
        }
    }
}
