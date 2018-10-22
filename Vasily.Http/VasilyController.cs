using System;

namespace Microsoft.AspNetCore.Mvc
{

    public class VasilyController<T> : VasilyResultController where T : class
    {
        protected DapperWrapper<T> SqlHandler;
        public VasilyController()
        {

        }
        public VasilyController(string key)
        {
            SqlHandler = new DapperWrapper<T>(key);
        }
        public VasilyController(string reader, string writter)
        {
            SqlHandler = new DapperWrapper<T>(reader, writter);
        }
    }

    public class VasilyController<T, R, S1> : VasilyResultController where T : class
    {
        protected RelationWrapper<T> SqlHandler;
        public VasilyController()
        {

        }

        public VasilyController(string key)
        {
            SqlHandler = new DapperWrapper<T, R, S1>(key);
        }
        public VasilyController(string reader, string writter)
        {
            SqlHandler = new DapperWrapper<T, R, S1>(reader, writter);
        }
    }
    public class VasilyController<T, R, S1, S2> : VasilyResultController where T : class
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

    public class VasilyController<T, R, S1, S2, S3> : VasilyResultController where T : class
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
    public class VasilyController<T, R, S1, S2, S3, S4> : VasilyResultController where T : class
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

    public class VasilyController<T, R, S1, S2, S3, S4, S5> : VasilyResultController where T : class
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
    public class VasilyController<T, R, S1, S2, S3, S4, S5, S6> : VasilyResultController where T : class
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
        protected ReturnPageResult Result(bool value, int totle, string succeed = "操作成功！", string faild = "操作失败！")
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
        protected ReturnResult Result(bool value, string succeed = "操作成功！", string faild = "操作失败！")
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
    public struct ReturnPageResult
    {
        public string Msg;
        public object Data;
        public int Status;
        public int Totle;
    }
    public struct ReturnResult
    {
        public string Msg;
        public object Data;
        public int Status;
    }
}
