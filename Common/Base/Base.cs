using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 操作基础类
    /// </summary>
    public class Base
    {
        /// <summary>
        /// 编码，1代表：成功  0：失败  -100：异常  -200 未登录或登录超时
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string Message { get; set; }

        #region 构造器
        /// <summary>
        /// 默认的空构造器
        /// </summary>
        public Base()
        {

        }
        /// <summary>
        /// 构造Base
        /// </summary>
        /// <param name="result">操作结果</param>
        /// <param name="message">信息</param>
        public Base(bool result, string message = null)
        {
            if (result)
            {
                Code = 1;
                this.Message = string.IsNullOrEmpty(message) ? "操作成功" : message;
            }
            else
            {
                Code = 0;
                this.Message = string.IsNullOrEmpty(message) ? "操作失败" : message;
            }
        }
        /// <summary>
        /// 构造Base
        /// </summary>
        /// <param name="code">操作编码</param>
        /// <param name="message">信息</param>
        public Base(int code, string message = null)
        {
            this.Code = code;
            this.Message = message;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 是否执行成功
        /// </summary>
        /// <returns></returns>
        public bool IsSuccess()
        {
            return Code == 1;
        }
        /// <summary>
        /// 是否执行失败
        /// </summary>
        /// <returns></returns>
        public bool IsFail()
        {
            return IsSuccess() == false;
        }
        /// <summary>
        /// 设置成功
        /// </summary>
        public Base setSuccess(string message = "")
        {
            this.Code = 1;
            if (message.IsNotNull())
                this.Message = message;
            return this;
        }
        /// <summary>
        /// 设置失败
        /// </summary>
        /// <param name="message">提示信息</param>
        public Base setFail(string message = "")
        {
            this.Code = 0;
            if (message.IsNotNull())
                this.Message = message;
            return this;
        }
        /// <summary>
        /// 设置失败
        /// </summary>
        /// <param name="code">失败的编码</param>
        /// <param name="message">提示信息</param>
        public Base setFail(int code = 0, string message = "")
        {
            this.Code = code;
            if (message.IsNotNull())
                this.Message = message;
            return this;
        }
        /// <summary>
        /// 设置失败
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="message">提示信息</param>
        public Base setFail(Exception e, string message = "")
        {
            this.Code = -100;
            Common.LogHelper.Error(message, e);
            return this;
        }
        #endregion

    }
    /// <summary>
    /// 操作基础类：带返回对象数据Data
    /// </summary>
    public class Base<T> : Base
    {
        /// <summary>
        /// 对象数据
        /// </summary>
        public T Data { get; set; }
        #region 构造器
        /// <summary>
        /// 默认的空构造器
        /// </summary>
        public Base()
        {

        }
        /// <summary>
        /// 构造Base
        /// </summary>
        /// <param name="result">操作结果</param>
        /// <param name="message">信息</param>
        public Base(bool result, string message = null)
        {
            if (result)
            {
                Code = 1;
                this.Message = string.IsNullOrEmpty(message) ? "操作成功" : message;
            }
            else
            {
                Code = 0;
                this.Message = string.IsNullOrEmpty(message) ? "操作失败" : message;
            }
        }
        /// <summary>
        /// 构造Base
        /// </summary>
        /// <param name="code">操作编码</param>
        /// <param name="message">信息</param>
        public Base(int code, string message = null)
        {
            this.Code = code;
            this.Message = message;
        }
        #endregion

        /// <summary>
        /// 设置成功
        /// </summary>
        public Base<T> setSuccess(string message = "", T data = default(T))
        {
            this.Code = 1;
            if (this.Data == null || this.Data.Equals(default(T)))
            {
                this.Data = data;
            }
            if (message.IsNotNull())
                this.Message = message;
            return this;
        }
        /// <summary>
        /// 设置失败
        /// </summary>
        /// <param name="message">失败的信息</param>
        /// <param name="data">失败的data</param>
        public Base<T> setFail(string message = "", T data = default(T))
        {
            this.Data = data;
            if (message.IsNotNull())
                this.Message = message;
            return this;
        }
        /// <summary>
        /// 设置失败
        /// </summary>
        /// <param name="code">失败的编码</param>
        /// <param name="message">失败的信息</param>
        /// <param name="data">失败的data</param>
        public Base<T> setFail(int code = 0, string message = "", T data = default(T))
        {
            this.Code = code;
            this.Data = data;
            if (message.IsNotNull())
                this.Message = message;
            return this;
        }
        /// <summary>
        /// 设置失败
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="message">提示信息</param>
        /// <param name="data">data</param>
        public Base<T> setFail(Exception e, string message = "", T data = default(T))
        {
            this.Code = -100;
            this.Data = data;
            this.Message = message;
            Common.LogHelper.Error(message, e);
            return this;
        }
    }
}
