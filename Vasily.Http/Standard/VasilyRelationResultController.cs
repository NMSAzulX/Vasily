using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Vasily.VP;

namespace Vasily.Http.Standard
{
    public class VasilyRelationResultController<T> : VasilyResultController where T : class
    {
        public RelationWrapper<T> driver;
        public SqlCondition<T> c;
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
        /// <param name="message">附加信息</param>
        /// <returns></returns>
        protected ReturnResult DeleteAftResult(object value, string message = "删除失败!")
        {
            return Result(driver.SourceAftDelete(value), message);
        }
        /// <summary>
        /// 前置删除操作的结果返回
        /// </summary>
        /// <param name="value">实例</param>
        /// <param name="message">附加信息</param>
        /// <returns></returns>
        protected ReturnResult DeletePreResult(object value, string message = "删除失败!")
        {
            return Result(driver.SourcePreDelete(value), message);
        }
        /// <summary>
        /// 添加操作的结果返回
        /// </summary>
        /// <param name="instances">实例与查询条件</param>
        /// <param name="message">附加信息</param>
        /// <returns></returns>
        protected ReturnResult AddResult(IEnumerable<T> instances, string message = "添加失败!")
        {
            return Result(driver.SourceAdd(instances), message);
        }
        /// <summary>
        /// 用条件查询实例结果集，用条件查询总数
        /// </summary>
        /// <param name="value">实例</param>
        /// <param name="message">附加信息</param>
        /// <returns></returns>
        protected ReturnPageResult GetsPageResult(object value, string message = "查询失败!")
        {
            return Result(driver.SourceGets(value), driver.SourceCount(value), message);
        }
        /// <summary>
        /// 用条件查询实例结果
        /// </summary>
        /// <param name="value">实例</param>
        /// <param name="message">附加信息</param>
        /// <returns></returns>
        protected ReturnResult GetResult(object value, string message = "查询失败!")
        {
            return Result(driver.SourceGet(value), message);
        }

        /// <summary>
        /// 用条件查询实例结果集
        /// </summary>
        /// <param name="value">实例</param>
        /// <param name="message">附加信息</param>
        /// <returns></returns>
        protected ReturnResult GetsResult(object value, string message = "查询失败!")
        {
            return Result(driver.SourceGets(value), message);
        }

        #endregion
    }
}
