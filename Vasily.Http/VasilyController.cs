using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Mvc
{

    public class VasilyController<T> : VasilyResultController where T : class
    {
        protected DapperWrapper<T> driver;
        protected SqlCondition<T> c;
        public VasilyController()
        {
            _union_tables = new string[0];
        }
        public VasilyController(string key):this()
        {
            driver = new DapperWrapper<T>(key);
            c = new SqlCondition<T>();
        }
        public VasilyController(string reader, string writter) : this()
        {
            driver = new DapperWrapper<T>(reader, writter);
            c = new SqlCondition<T>();
        }
        private string[] _union_tables;
        public void UseUnion(params string[] unions)
        {
            driver.Unions = unions;
        }

        [HttpPost("query--page-vp")]
        public ReturnPageResult QueryPageVP(SqlVP<T> vp_instance)
        {
            return GetsPageResult(vp_instance);
        }

        [HttpPost("query-vp")]
        public ReturnResult QueryVP(SqlVP<T> vp_instance)
        {
            return GetResult(vp_instance);
        }




        #region NormalQuery
        [HttpGet("vasily")]
        public ReturnPageResult VasilyGet(SqlVP<T> vp_instance)
        {
            return GetsPageResult(vp_instance);
        }
        [HttpPost("vasily")]
        public ReturnResult VasilyModify(params T[] instances)
        {
            return ModifyResult(instances);
        }
        [HttpPut("vasily")]
        public ReturnResult VasilyAdd(params T[] instances)
        {
            return AddResult(instances);
        }
        [HttpDelete("vasily")]
        public ReturnResult VasilyDelete(params T[] instances)
        {
            return DeleteResult(instances);
        }
        #endregion


        #region 信息返回封装
        /// <summary>
        /// 更新操作的结果返回
        /// </summary>
        /// <param name="cp">实例与查询条件</param>
        /// <param name="msg">附加信息</param>
        /// <returns></returns>
        protected ReturnResult ModifyResult(SqlCP<T> cp, string msg = "更新失败!")
        {
            return Result(driver.Modify(cp), msg);
        }
        protected ReturnResult ModifyResult(SqlCP cp, string msg = "更新失败!")
        {
            return Result(driver.Modify(cp), msg);
        }
        protected ReturnResult ModifyResult(T[] instances, string msg = "删除失败!")
        {
            return Result(driver.ModifyByPrimary(instances), msg);
        }
        /// <summary>
        /// 删除操作的结果返回
        /// </summary>
        /// <param name="cp">实例与查询条件</param>
        /// <param name="msg">附加信息</param>
        /// <returns></returns>
        protected ReturnResult DeleteResult(SqlCP<T> cp, string msg = "删除失败!", ForceDelete flag = ForceDelete.No )
        {
            return Result(driver.Delete(cp, flag), msg);
        }
        protected ReturnResult DeleteResult(SqlCP cp, string msg = "删除失败!", ForceDelete flag = ForceDelete.No)
        {
            return Result(driver.Delete(cp, flag), msg);
        }
        protected ReturnResult DeleteResult(T[] instances, string msg = "删除失败!")
        {
            return Result(driver.EntitiesDeleteByPrimary(instances), msg);
        }
        /// <summary>
        /// 添加操作的结果返回
        /// </summary>
        /// <param name="instances">实例</param>
        /// <param name="msg">附加信息</param>
        /// <returns></returns>
        protected ReturnResult AddResult(IEnumerable<T> instances, string msg = "添加失败!")
        {
            return Result(driver.Add(instances), msg);
        }
        protected ReturnResult AddResult(T[] instances, string msg = "添加失败!")
        {
            return Result(driver.Add(instances), msg);
        }
        /// <summary>
        /// 安全添加操作的结果返回
        /// </summary>
        /// <param name="instances">实例</param>
        /// <param name="msg">附加信息</param>
        /// <returns></returns>
        protected ReturnResult AddResult(T instances, string msg = "添加失败!")
        {
            return Result(driver.SafeAdd(instances), msg);
        }
        /// <summary>
        /// 用条件查询实例结果集，用条件查询总数
        /// </summary>
        /// <param name="cp_value">实例与查询条件</param>
        /// <param name="cp_count">实例与查询条件</param>
        /// <param name="msg">附加信息</param>
        /// <returns></returns>
        protected ReturnPageResult GetsPageResult(SqlCP cp_value, string msg = "查询失败!")
        {
            return Result(driver.Gets(cp_value), driver.CountWithCondition(cp_value), msg);
        }
        protected ReturnPageResult GetsPageResult(SqlCP<T> cp_value, string msg = "查询失败!")
        {
            return Result(driver.Gets(cp_value), driver.CountWithCondition(cp_value), msg);
        }
        /// <summary>
        /// 用条件查询实例结果
        /// </summary>
        /// <param name="cp">实例与查询条件</param>
        /// <param name="msg">附加信息</param>
        /// <returns></returns>
        protected ReturnResult GetResult(SqlCP<T> cp, string msg = "查询失败!")
        {
            return Result(driver.Get(cp), msg);
        }
        protected ReturnResult GetResult(SqlCP cp, string msg = "查询失败!")
        {
            return Result(driver.Get(cp), msg);
        }
        /// <summary>
        /// 用条件查询实例结果集
        /// </summary>
        /// <param name="cp">实例与查询条件</param>
        /// <param name="msg">附加信息</param>
        /// <returns></returns>
        protected ReturnResult GetsResult(SqlCP<T> cp, string msg = "查询失败!")
        {
            return Result(driver.Gets(cp), msg);
        }
        protected ReturnResult GetsResult(SqlCP cp, string msg = "查询失败!")
        {
            return Result(driver.Gets(cp), msg);
        }
        #endregion
    }


    public class VasilyResultController<T> : VasilyResultController where T : class
    {

        protected RelationWrapper<T> driver;
        protected SqlCondition<T> c;
        public VasilyResultController()
        {
            c = new SqlCondition<T>();
        }
        #region 信息返回封装
        /// <summary>
        /// 更新操作的结果返回
        /// </summary>
        /// <param name="value">实例</param>
        /// <param name="msg">附加信息</param>
        /// <returns></returns>
        protected ReturnResult ModifyResult(object value, string msg = "更新失败!")
        {
            return Result(driver.SourceModify(value), msg);
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

    public class VasilyController<T, R, S1> : VasilyResultController<T> where T : class
    {

        public VasilyController()
        {

        }

        public VasilyController(string key) : base()
        {
            driver = new DapperWrapper<T, R, S1>(key);

        }
        public VasilyController(string reader, string writter) : base()
        {
            driver = new DapperWrapper<T, R, S1>(reader, writter);
        }


    }
    public class VasilyController<T, R, S1, S2> : VasilyResultController<T> where T : class
    {
        protected RelationWrapper<T> SqlHandler;
        public VasilyController()
        {

        }

        public VasilyController(string key)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2>(key);
        }
        public VasilyController(string reader, string writter)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2>(reader, writter);
        }

    }

    public class VasilyController<T, R, S1, S2, S3> : VasilyResultController<T> where T : class
    {
        protected RelationWrapper<T> SqlHandler;
        public VasilyController()
        {

        }

        public VasilyController(string key)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2, S3>(key);
        }
        public VasilyController(string reader, string writter)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2, S3>(reader, writter);
        }

    }
    public class VasilyController<T, R, S1, S2, S3, S4> : VasilyResultController<T> where T : class
    {
        protected RelationWrapper<T> SqlHandler;
        public VasilyController()
        {

        }

        public VasilyController(string key)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2, S3, S4>(key);
        }
        public VasilyController(string reader, string writter)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2, S3, S4>(reader, writter);
        }

    }

    public class VasilyController<T, R, S1, S2, S3, S4, S5> : VasilyResultController<T> where T : class
    {
        protected RelationWrapper<T> SqlHandler;
        public VasilyController()
        {

        }

        public VasilyController(string key)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2, S3, S4, S5>(key);
        }
        public VasilyController(string reader, string writter)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2, S3, S4, S5>(reader, writter);
        }

    }
    public class VasilyController<T, R, S1, S2, S3, S4, S5, S6> : VasilyResultController<T> where T : class
    {
        protected RelationWrapper<T> SqlHandler;
        public VasilyController()
        {

        }

        public VasilyController(string key)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2, S3, S4, S5, S6>(key);
        }
        public VasilyController(string reader, string writter)
        {
            SqlHandler = new DapperWrapper<T, R, S1, S2, S3, S4, S5, S6>(reader, writter);
        }
    }

    public class VasilyResultController : ControllerBase
    {

        /// <summary>
        /// 分页 - 用布尔类型操作返回值
        /// </summary>
        /// <param name="value">true/false代表返回成功与否</param>
        /// <param name="totle">总条数</param>
        /// <param name="succeed">正确提示，默认：操作成功！</param>
        /// <param name="faild">错误提示，默认：操作失败！</param>
        /// <returns></returns>
        protected ReturnPageResult BoolResult(bool value, int totle, string succeed = "操作成功！", string faild = "操作失败！")
        {
            ReturnPageResult _result = new ReturnPageResult();
            _result.Totle = totle;
            if (value)
            {
                _result.Status = 0;
                _result.Msg = succeed;
            }
            else
            {
                _result.Status = 1;
                _result.Msg = faild;
            }
            return _result;
        }
        /// <summary>
        /// 分页 - 返回对象，若对象为空，则返回错误信息
        /// </summary>
        /// <param name="value">需要传送的对象</param>
        /// <param name="totle"></param>
        /// <param name="msg">错误提示信息</param>
        /// <returns></returns>
        protected ReturnPageResult Result(object value, int totle, string msg = "数据为空！")
        {
            ReturnPageResult _result = new ReturnPageResult();
            _result.Totle = totle;
            if (value != null)
            {
                _result.Data = value;
            }
            else
            {
                _result.Status = 1;
                _result.Msg = msg;
            }
            return _result;
        }
        /// <summary>
        /// 用布尔类型操作返回值
        /// </summary>
        /// <param name="value">true/false代表返回成功与否</param>
        /// <param name="succeed">正确提示，默认：操作成功！</param>
        /// <param name="faild">错误提示，默认：操作失败！</param>
        /// <returns></returns>
        protected ReturnResult BoolResult(bool value, string succeed = "操作成功！", string faild = "操作失败！")
        {
            ReturnResult _result = new ReturnResult();
            if (value)
            {
                _result.Status = 0;
                _result.Msg = succeed;
            }
            else
            {
                _result.Status = 1;
                _result.Msg = faild;
            }
            return _result;
        }
        /// <summary>
        /// 返回对象，若对象为空，则返回错误信息
        /// </summary>
        /// <param name="value">需要传送的对象</param>
        /// <param name="msg">错误提示信息</param>
        /// <returns></returns>
        protected ReturnResult Result(object value, string msg = "数据为空！")
        {
            ReturnResult _result = new ReturnResult();
            if (value != null)
            {
                _result.Data = value;
            }
            else
            {
                _result.Status = 1;
                _result.Msg = msg;
            }
            return _result;
        }
        /// <summary>
        /// 返回提示信息
        /// </summary>
        /// <param name="value">提示信息</param>
        /// <param name="status">状态码，默认200</param>
        /// <returns></returns>
        protected ReturnResult Msg(string value, int status = 200)
        {
            ReturnResult _result = new ReturnResult();
            _result.Msg = value;
            _result.Status = status;
            return _result;
        }
    }



}
