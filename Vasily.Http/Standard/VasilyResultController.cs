using Microsoft.AspNetCore.Mvc;

namespace Vasily.Http.Standard
{

    public class VasilyResultController : ControllerBase
    {

        /// <summary>
        /// 分页 - 用布尔类型操作返回值
        /// </summary>
        /// <param name="value">true/false代表返回成功与否</param>
        /// <param name="totle">总条数</param>
        /// <param name="message">正确提示，默认：操作成功！错误提示，默认：操作失败！</param>
        /// <returns></returns>
        protected ReturnPageResult BoolResult(bool value, int totle, string message = null)
        {
            ReturnPageResult _result = new ReturnPageResult();
            _result.totle = totle;
            if (value)
            {
                _result.code = 0;
                _result.message = message==null?"操作成功！":message;
            }
            else
            {
                _result.code = 1;
                _result.message = message == null ? "操作失败！" : message;
            }
            return _result;
        }
        /// <summary>
        /// 分页 - 返回对象，若对象为空，则返回错误信息
        /// </summary>
        /// <param name="value">需要传送的对象</param>
        /// <param name="totle"></param>
        /// <param name="message">正确提示，默认：操作成功！错误提示，默认：操作失败！</param>
        /// <returns></returns>
        protected ReturnPageResult Result(object value, int totle, string message = null)
        {
            ReturnPageResult _result = new ReturnPageResult();
            _result.totle = totle;
            if (value != null)
            {
                _result.data = value;
                _result.code = 0;
                _result.message = message == null ? "操作成功！" : message;
            }
            else
            {
                _result.code = 1;
                _result.message = message == null ? "操作失败！" : message;
            }
            return _result;
        }
        /// <summary>
        /// 用布尔类型操作返回值
        /// </summary>
        /// <param name="value">true/false代表返回成功与否</param>
        /// <param name="message">正确提示，默认：操作成功！错误提示，默认：操作失败！</param
        /// <returns></returns>
        protected ReturnResult BoolResult(bool value, string message = null)
        {
            ReturnResult _result = new ReturnResult();
            if (value)
            {
                _result.code = 0;
                _result.message = message == null ? "操作成功！" : message;
            }
            else
            {
                _result.code = 1;
                _result.message = message == null ? "操作失败！" : message;
            }
            return _result;
        }
        /// <summary>
        /// 返回对象，若对象为空，则返回错误信息
        /// </summary>
        /// <param name="value">需要传送的对象</param>
        /// <param name="message">正确提示，默认：操作成功！错误提示，默认：操作失败！</param
        /// <returns></returns>
        protected ReturnResult Result(object value, string message = null)
        {
            ReturnResult _result = new ReturnResult();
            if (value != null)
            {
                _result.data = value;
                _result.code = 0;
                _result.message = message == null ? "操作成功！" : message;
            }
            else
            {
                _result.code = 1;
                _result.message = message == null ? "操作失败！" : message;
            }
            return _result;
        }
        /// <summary>
        /// 返回提示信息
        /// </summary>
        /// <param name="value">提示信息</param>
        /// <param name="status">状态码，默认1</param>
        /// <returns></returns>
        public ReturnResult Error(string value, int status = 1)
        {
            ReturnResult _result = new ReturnResult();
            _result.message = value;
            _result.code = status;
            return _result;
        }
        /// <summary>
        /// 返回提示信息
        /// </summary>
        /// <param name="value">提示信息</param>
        /// <param name="status">状态码，默认1</param>
        /// <returns></returns>
        public ReturnResult Error(int status,string value)
        {
            ReturnResult _result = new ReturnResult();
            _result.message = value;
            _result.code = status;
            return _result;
        }
    }
}
