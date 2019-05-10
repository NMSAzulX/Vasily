using System;
using System.Collections.Generic;
using Vasily;
using Vasily.Http.Standard;
using Vasily.VP;

namespace Microsoft.AspNetCore.Mvc
{
    public class VasilyController<T> : VasilyNormalResultController<T> where T : class
    {
        public VasilyController()
        {
        }
        public VasilyController(string key) : base(key)
        {
        }
        public VasilyController(string reader, string writter) : base(reader, writter)
        {

        }
        #region 集合操作
        public void UseUnion(params string[] tables)
        {
            driver.UseCollection(SqlCollectionType.Union, tables);
        }
        public void UseExcept(params string[] tables)
        {
            driver.UseCollection(SqlCollectionType.Except, tables);
        }
        public void UseIntersect(params string[] tables)
        {
            driver.UseCollection(SqlCollectionType.Intersect, tables);
        }
        public void UseUnionAll(params string[] tables)
        {
            driver.UseCollection(SqlCollectionType.UnionAll, tables);
        }
        #endregion



    }
}
