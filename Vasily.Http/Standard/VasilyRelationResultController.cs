using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Vasily.VP;

namespace Vasily.Http.Standard
{
    public class VasilyRelationResultController<T> : VasilyResultController where T : class
    {

        protected RelationWrapper<T> driver;
        protected SqlCondition<T> c;
        public VasilyRelationResultController()
        {
            c = new SqlCondition<T>();
        }
        #region 信息返回封装
        /// <summary>
        /// 更新操作的结果返回
        /// </summary>
        /// <param name="value">实例</param>
        /// <param name="message">附加信息</param>
        /// <returns></returns>
        protected ReturnResult ModifyResult(object value, string message = "更新失败!")
        {
            return Result(driver.SourceModify(value), message);
        }
        /// <summary>
        /// 后置删除操作的结果返回
        /// </summary>
        /// <param name="value">实例</param>
        /// <param name="msg">附加信息</param>
        /// <returns></returns>
        protected ReturnResult DeleteAftResult(object value, string msg = "删除失败!")
        {
            return Result(driver.SourceAftDelete(value), msg);
        }
        /// <summary>
        /// 前置删除操作的结果返回
        /// </summary>
        /// <param name="value">实例</param>
        /// <param name="msg">附加信息</param>
        /// <returns></returns>
        protected ReturnResult DeletePreResult(object value, string msg = "删除失败!")
        {
            return Result(driver.SourcePreDelete(value), msg);
        }
        /// <summary>
        /// 添加操作的结果返回
        /// </summary>
        /// <param name="instances">实例与查询条件</param>
        /// <param name="msg">附加信息</param>
        /// <returns></returns>
        protected ReturnResult AddResult(IEnumerable<T> instances, string msg = "添加失败!")
        {
            return Result(driver.SourceAdd(instances), msg);
        }
        /// <summary>
        /// 用条件查询实例结果集，用条件查询总数
        /// </summary>
        /// <param name="value">实例</param>
        /// <param name="msg">附加信息</param>
        /// <returns></returns>
        protected ReturnPageResult GetsPageResult(object value, string msg = "查询失败!")
        {
            return Result(driver.SourceGets(value), driver.SourceCount(value), msg);
        }
        /// <summary>
        /// 用条件查询实例结果
        /// </summary>
        /// <param name="value">实例</param>
        /// <param name="msg">附加信息</param>
        /// <returns></returns>
        protected ReturnResult GetResult(object value, string msg = "查询失败!")
        {
            return Result(driver.SourceGet(value), msg);
        }

        /// <summary>
        /// 用条件查询实例结果集
        /// </summary>
        /// <param name="value">实例</param>
        /// <param name="msg">附加信息</param>
        /// <returns></returns>
        protected ReturnResult GetsResult(object value, string msg = "查询失败!")
        {
            return Result(driver.SourceGets(value), msg);
        }

        #endregion
    }
}
