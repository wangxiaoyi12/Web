using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace DataBase
{
    /// <summary>
    /// 数据库连接字符串提取操作
    /// 字符串对应应用程序中配置文件
    /// 模型对应DataBase中的数据库模型Context.cs构造函数
    /// </summary>
    public class MyConfig : DbMallEntities //注：Entities要修改成与EF上下文统一
    {
        #region 连接数据库配置
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string DefaultConnectionString = "";
        /// <summary>
        /// 通用数据库链接对象配置
        /// </summary>
        public static IDbConnection DefaultConnection
        {
            get
            {
                IDbConnection defaultConn = null;
                defaultConn = new System.Data.SqlClient.SqlConnection();
                DefaultConnectionString = ConfigurationManager.ConnectionStrings["sqlConn"].ConnectionString;
                return defaultConn;
            }
        }
        /// <summary>
        /// 构造数据库连接字符串 注：数据库切换要修改
        /// </summary>
        public static string DataBaseConnectionString(string EntityName)
        {
            IDbConnection con = DefaultConnection;
            return EFConnectionStringModle(EntityName, DefaultConnectionString);
        }
        /// <summary>
        /// 构造EF使用数据库连接字符串
        /// </summary>
        /// <param name="EntityName">数据上下文坏境</param>
        /// <param name="DBsoure">数据字符串</param>
        static string EFConnectionStringModle(string EntityName, string DBsoure)
        {
            return string.Concat("metadata=res://*/",
                EntityName, ".csdl|res://*/",
                EntityName, ".ssdl|res://*/",
                EntityName, ".msl;provider=Oracle.ManagedDataAccess.Client;provider connection string='",
                DBsoure, "'");

        }
        #endregion        
    }
}
