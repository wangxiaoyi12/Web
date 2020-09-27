using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat
{
    /// <summary>
    /// 日志记录操作
    /// </summary>
    public class LogOperate
    {
        //static LogHelper.LogHelper _log = createLog();

        //public static LogHelper.LogHelper createLog()
        //{
        //    if (Directory.Exists("d:\\"))
        //        return new LogHelper.LogHelper("d:\\log.txt", true);
        //    return new LogHelper.LogHelper("e:\\log.txt", true);
        //}

        /// <summary>
        /// 写入内容标识
        /// </summary>
        /// <param name="content"></param>
        public static void Write(string content)
        {
            // LogHelper.Log(content);
            LogHelper.Debug(content);
            //_log.WriteLine(content);
        }




        /// <summary>
        /// 写入内容标识
        /// </summary>
        /// <param name="content"></param>
        public static void Write(Exception ex)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("错误详情：");
            while (ex != null)
            {
                builder.AppendLine(ex.Message);
                //进入下一次循环
                ex = ex.InnerException;
            }

            LogHelper.Debug(builder.ToString());
        }

    }
}
