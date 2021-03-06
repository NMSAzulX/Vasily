﻿using Vasily;
using Vasily.VP;

namespace Microsoft.AspNetCore.Mvc
{

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

        #region LinkQuery
        [HttpPost("accurate-count")]
        public ReturnResult VasilyLinkCount(VasilyProtocal<T> vp)
        {
            return Result(SqlLink<T>.Load(driver).Fields(vp.Fields).Conditions(vp).Count(vp.Instance));
        }
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
