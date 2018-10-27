using System;
using System.Collections.Generic;
using System.Text;
using VasilyUT.Entity;
using Xunit;

namespace VasilyUT
{
    public class UnitTest_VasilyUniqueId
    {
        [Fact(DisplayName = "一打雪花，带你勇闯天涯")]
        public void TestId()
        {
            HashSet<long> hashSet = new HashSet<long>();
            Snowflake<Student>.SetNodesInfo(54321, 12345);
            for (int i = 0; i < 1000; i+=1)
            {
                long result = Snowflake<Student>.NextId;
                Assert.DoesNotContain(result, hashSet);
                hashSet.Add(result);
            }
        }
    }
}
