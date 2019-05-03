using Vasily;
using Vasily.VP;

namespace Microsoft.AspNetCore.Mvc
{
    public class VPReadController<T> : VasilyController<T> where T : class
    {
        public VPReadController()
        {
        }
        public VPReadController(string key) : base(key)
        {

        }
        public VPReadController(string reader, string writter) : base(reader, writter)
        {

        }
        [HttpPost("query-page-vp")]
        public ReturnPageResult QueryPageVP(VasilyProtocal<T> vp)
        {
            return GetsPageResult(vp);
        }

        [HttpPost("query-vp")]
        public ReturnResult QueryVP(VasilyProtocal<T> vp)
        {
            return GetResult(vp);
        }

        #region LinkQuery
        [HttpPost("accurate-get")]
        public ReturnResult VasilyLinkGet(VasilyProtocal<T> vp)
        {
            return Result(SqlLink<T>.Load(driver).Fields(vp.Fields).Conditions(vp).Get<object>(vp.Instance));
        }
        [HttpPost("accurate-gets")]
        public ReturnResult VasilyLinkGets(VasilyProtocal<T> vp)
        {
            return Result(SqlLink<T>.Load(driver).Fields(vp.Fields).Conditions(vp).Gets<object>(vp.Instance));
        }
        [HttpPost("accurate-page-gets")]
        public ReturnPageResult VasilyLinkPageGets(VasilyProtocal<T> vp)
        {
            return Result(SqlLink<T>.Load(driver).Fields(vp.Fields).Conditions(vp).Gets<object>(vp.Instance), driver.CountWithCondition(vp));
        }
        [HttpDelete("accurate-repeate")]
        public ReturnResult VasilyLinkRepeate(VasilyProtocal<T> vp)
        {
            return Result(SqlLink<T>.Load(driver).Fields(vp.Fields).Conditions(vp).Repeate(vp.Instance));
        }
        #endregion
    }

    public class VPWriteController<T> : VasilyController<T> where T : class
    {
        public VPWriteController()
        {
        }
        public VPWriteController(string key) : base(key)
        {

        }
        public VPWriteController(string reader, string writter) : base(reader, writter)
        {

        }
     

        #region LinkQuery
        
        [HttpPost("accurate-modify")]
        public ReturnResult VasilyLinkModify(VasilyProtocal<T> vp)
        {
            return Result(SqlLink<T>.Load(driver).Fields(vp.Fields).Conditions(vp).Modify(vp.Instance));
        }
        [HttpPut("accurate-add")]
        public ReturnResult VasilyLinkAdd(VasilyProtocal<T> vp)
        {
            return Result(SqlLink<T>.Load(driver).Fields(vp.Fields).Conditions(vp).Add(vp.Instance));
        }
        [HttpDelete("accurate-delete")]
        public ReturnResult VasilyLinkDelete(VasilyProtocal<T> vp)
        {
            return Result(SqlLink<T>.Load(driver).Fields(vp.Fields).Conditions(vp).Delete(vp.Instance));
        }
        #endregion


        #region 整个实体进行操作
        [HttpPost("all-modify")]
        public ReturnResult VasilyModify(params T[] instances)
        {
            return ModifyResult(instances);
        }
        [HttpPut("all-add")]
        public ReturnResult VasilyAdd(params T[] instances)
        {
            return AddResult(instances);
        }
        [HttpDelete("all-delete")]
        public ReturnResult VasilyDelete(params T[] instances)
        {
            return DeleteResult(instances);
        }
        #endregion
    }

    public class VPReadAndWriteController<T> : VasilyController<T> where T : class
    {
        public VPReadAndWriteController()
        {
        }
        public VPReadAndWriteController(string key) : base(key)
        {

        }
        public VPReadAndWriteController(string reader, string writter) : base(reader, writter)
        {

        }
        [HttpPost("query-page-vp")]
        public ReturnPageResult QueryPageVP(VasilyProtocal<T> vp)
        {
            return GetsPageResult(vp);
        }

        [HttpPost("query-vp")]
        public ReturnResult QueryVP(VasilyProtocal<T> vp)
        {
            return GetResult(vp);
        }

        #region LinkQuery
        [HttpPost("accurate-get")]
        public ReturnResult VasilyLinkGet(VasilyProtocal<T> vp)
        {
            return Result(SqlLink<T>.Load(driver).Fields(vp.Fields).Conditions(vp).Get<object>(vp.Instance));
        }
        [HttpPost("accurate-gets")]
        public ReturnResult VasilyLinkGets(VasilyProtocal<T> vp)
        {
            return Result(SqlLink<T>.Load(driver).Fields(vp.Fields).Conditions(vp).Gets<object>(vp.Instance));
        }
        [HttpPost("accurate-page-gets")]
        public ReturnPageResult VasilyLinkPageGets(VasilyProtocal<T> vp)
        {
            return Result(SqlLink<T>.Load(driver).Fields(vp.Fields).Conditions(vp).Gets<object>(vp.Instance), driver.CountWithCondition(vp));
        }
        [HttpDelete("accurate-repeate")]
        public ReturnResult VasilyLinkRepeate(VasilyProtocal<T> vp)
        {
            return Result(SqlLink<T>.Load(driver).Fields(vp.Fields).Conditions(vp).Repeate(vp.Instance));
        }
        [HttpPost("accurate-modify")]
        public ReturnResult VasilyLinkModify(VasilyProtocal<T> vp)
        {
            return Result(SqlLink<T>.Load(driver).Fields(vp.Fields).Conditions(vp).Modify(vp.Instance));
        }
        [HttpPut("accurate-add")]
        public ReturnResult VasilyLinkAdd(VasilyProtocal<T> vp)
        {
            return Result(SqlLink<T>.Load(driver).Fields(vp.Fields).Conditions(vp).Add(vp.Instance));
        }
        [HttpDelete("accurate-delete")]
        public ReturnResult VasilyLinkDelete(VasilyProtocal<T> vp)
        {
            return Result(SqlLink<T>.Load(driver).Fields(vp.Fields).Conditions(vp).Delete(vp.Instance));
        }
        #endregion

        #region 整个实体进行操作
        [HttpPost("all-modify")]
        public ReturnResult VasilyModify(params T[] instances)
        {
            return ModifyResult(instances);
        }
        [HttpPut("all-add")]
        public ReturnResult VasilyAdd(params T[] instances)
        {
            return AddResult(instances);
        }
        [HttpDelete("all-delete")]
        public ReturnResult VasilyDelete(params T[] instances)
        {
            return DeleteResult(instances);
        }
        #endregion
    }
}
 