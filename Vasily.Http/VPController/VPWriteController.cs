using Vasily;
using Vasily.VP;

namespace Microsoft.AspNetCore.Mvc
{
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
}
