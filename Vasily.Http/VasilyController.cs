using System;
using System.Collections.Generic;
using Vasily;
using Vasily.Http.Standard;
using Vasily.VP;

namespace Microsoft.AspNetCore.Mvc
{
    public class VasilyController<T> : VasilyResultController where T : class
    {
        protected DapperWrapper<T> driver;
        protected SqlCondition<T> c;
        public VasilyController()
        {
        }
        public VasilyController(string key) : this()
        {
            driver = new DapperWrapper<T>(key);
            c = new SqlCondition<T>();
        }
        public VasilyController(string reader, string writter) : this()
        {
            driver = new DapperWrapper<T>(reader, writter);
            c = new SqlCondition<T>();
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



        #region 信息返回封装,对内的
        /// <summary>
        /// 更新操作的结果返回
        /// </summary>
        /// <param name="cp">实例与查询条件</param>
        /// <param name="message">附加信息</param>
        /// <returns></returns>
        protected ReturnResult ModifyResult(VasilyProtocal<T> cp, string message = "更新失败!")
        {
            return Result(driver.Modify(cp), message);
        }
        protected ReturnResult ModifyResult(T[] instances, string message = "删除失败!")
        {
            return Result(driver.ModifyByPrimary(instances), message);
        }
        /// <summary>
        /// 删除操作的结果返回
        /// </summary>
        /// <param name="cp">实例与查询条件</param>
        /// <param name="message">附加信息</param>
        /// <returns></returns>
        protected ReturnResult DeleteResult(VasilyProtocal<T> vp, string message = "删除失败!", ForceDelete flag = ForceDelete.No)
        {
            return Result(driver.Delete(vp, flag), message);
        }
        protected ReturnResult DeleteResult(T[] instances, string message = "删除失败!")
        {
            return Result(driver.EntitiesDeleteByPrimary(instances), message);
        }
        /// <summary>
        /// 添加操作的结果返回
        /// </summary>
        /// <param name="instances">实例</param>
        /// <param name="message">附加信息</param>
        /// <returns></returns>
        protected ReturnResult AddResult(IEnumerable<T> instances, string message = "添加失败!")
        {
            return Result(driver.Add(instances), message);
        }
        protected ReturnResult AddResult(T[] instances, string message = "添加失败!")
        {
            return Result(driver.Add(instances), message);
        }
        /// <summary>
        /// 安全添加操作的结果返回
        /// </summary>
        /// <param name="instances">实例</param>
        /// <param name="message">附加信息</param>
        /// <returns></returns>
        protected ReturnResult AddResult(T instances, string message = "添加失败!")
        {
            return Result(driver.SafeAdd(instances), message);
        }

        /*
        /// <summary>
        /// 用条件查询实例结果集，用条件查询总数
        /// </summary>
        /// <param name="vp">实例与查询条件</param>
        /// <param name="cp_count">实例与查询条件</param>
        /// <param name="message">附加信息</param>
        /// <returns></returns>
        protected ReturnPageResult GetsPageResult(VasilyProtocal<T> vp, string message = "查询失败!")
        {
            return Result(driver.Gets(vp), driver.CountWithCondition(vp), message);
        }
        /// <summary>
        /// 用条件查询实例结果
        /// </summary>
        /// <param name="cp">实例与查询条件</param>
        /// <param name="message">附加信息</param>
        /// <returns></returns>
        protected ReturnResult GetResult(VasilyProtocal<T> vp, string message = "查询失败!")
        {
            return Result(driver.Get(vp), message);
        }
        /// <summary>
        /// 用条件查询实例结果集
        /// </summary>
        /// <param name="cp">实例与查询条件</param>
        /// <param name="message">附加信息</param>
        /// <returns></returns>
        protected ReturnResult GetsResult(VasilyProtocal<T> vp, string message = "查询失败!")
        {
            return Result(driver.Gets(vp), message);
        }*/
        #endregion
    }
}
