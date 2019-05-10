using Vasily;
using Vasily.VP;

namespace Microsoft.AspNetCore.Mvc
{
    public class VPCountController<T> : VasilyController<T> where T : class
    {
        #region LinkQuery
        [HttpPost("accurate-count")]
        public ReturnResult VasilyLinkGet(VasilyProtocal<T> vp)
        {
            return Result(SqlLink<T>.Load(driver).Fields(vp.Fields).Conditions(vp).Count(vp.Instance));
        }
        #endregion
    }
}
