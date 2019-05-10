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
        #endregion
    }
}
