using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using Common;

namespace Common
{
    /// <summary>
    /// 数据库常用操作封装
    /// 功能说明：
    /// 1.创建完整数据库备份，到网站根目录下的backup文件夹
    /// 2.完整还原数据库
    /// 3.访问备份文件列表，删除备份文件
    /// 4.执行清空业务数据存储过程
    /// 特别说明
    /// 1.此处备份使用单用户模式，解决备份时‘数据库不能独占问题’
    /// </summary>
    public class DataBaseHelper
    {
        #region 数据库操作
        /// <summary>
        /// 清空业务数据
        /// </summary>
        public static void ClearData()
        {
            try
            {
                string sql = "exec [dbo].[proc_ClearData]";
                DbHelperSQL.ExecuteSql(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("清空数据库数据失败", ex);
            }
        }
        /// <summary>
        /// 创建数据库备份
        /// </summary>
        public static void CreateBackup(string before_string = "")
        {
            string dbname = GetDbName();
            //要备份的位置
            string dbfullname = GetDbPath() + before_string + string.Format("{0}_{1}.bak", dbname, DateTime.Now.ToString("yyyy_MM_dd_hhmmss"));
            //判断文件是否存在
            if (File.Exists(dbfullname))
            {
                throw new Exception(dbfullname + "的备份文件已经存在，请稍后再试");
            }
            SqlConnection con = DbHelperSQL.GetConnection();
            SqlCommand cmd = con.CreateCommand();
            con.Open();
            try
            {
                cmd.CommandText = "use master";
                cmd.ExecuteNonQuery();

                //1. 执行备份操作
                StringBuilder sql = new StringBuilder();
                //sql.Append("exec master.dbo.proc_Backup @dbName,@dbFullName");

                sql.Append(@"DECLARE @kid varchar(100)  
                        SET @kid=''  
                        SELECT @kid=@kid+'KILL '+CAST(spid as Varchar(10))  FROM master..sysprocesses  
                        WHERE dbid=DB_ID(@dbName)  
                        PRINT @kid  
                        EXEC(@kid);
                        backup database @dbName to disk=@dbFullName;");


                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("@dbName",SqlDbType.NVarChar,200),
                new SqlParameter("@dbFullName",SqlDbType.NVarChar,200),
            };
                parameters[0].Value = dbname;
                parameters[1].Value = dbfullname;

                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(parameters);
                cmd.CommandText = sql.ToString();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("创建数据库备份出错：" + ex);
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// 还原数据库
        /// </summary>
        public static void Restore(string restoreFileName)
        {
            //1.获取还原数据库和文件
            string dbName = GetDbName();
            string dbFullName = GetDbPath() + restoreFileName;
            try
            {
                //2.执行还原操作
                SqlConnection con = DbHelperSQL.GetConnection();
                SqlCommand cmd = con.CreateCommand();
                con.Open();
                try
                {
                    cmd.CommandText = "use master";
                    cmd.ExecuteNonQuery();

                    StringBuilder sql = new StringBuilder();
                    //sql.Append("exec proc_Restore @dbFullName,@dbName");

                    sql.Append(@"--1.1修改为单用模式
                     exec(N'ALTER DATABASE '+@dbName+' SET SINGLE_USER WITH ROLLBACK IMMEDIATE');
                    --1.2结束链接进程
	                    DECLARE @kid varchar(max)  
	                    SET @kid=''  
	                    SELECT @kid=@kid+'KILL '+CAST(spid as Varchar(10))  FROM master..sysprocesses  
	                    WHERE dbid=DB_ID(@dbName)  ;
	                    EXEC(@kid) ;
                    --2.执行还原语句
                       restore database @dbName from  disk=@dbFullName
                       with replace  --覆盖现有的数据库
                    --3.重置数据库为多用户模式
                     exec(N'ALTER DATABASE '+@dbName+' SET MULTI_USER WITH ROLLBACK IMMEDIATE');");
                    SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@dbName",SqlDbType.NVarChar,200),
                    new SqlParameter("@dbFullName",SqlDbType.NVarChar,200),
                };
                    parameters[0].Value = dbName;
                    parameters[1].Value = dbFullName;

                    cmd.CommandText = sql.ToString();
                    cmd.Parameters.AddRange(parameters);
                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("还原数据库出错" + ex);
            }
        }
        #endregion

        #region 备份文件操作
        /// <summary>
        /// 获取当前备份文件列表
        /// </summary>
        /// <returns></returns>
        public static List<FileInfo> GetList_Bak()
        {
            string path = GetDbPath();
            return GetDirectoryFiles(path, "bak");
        }
        /// <summary>
        /// 删除备份文件
        /// </summary>
        /// <param name="bakFileName"></param>
        /// <returns></returns>
        public static bool Delete_Bak(string bakFileName)
        {
            string fullname = GetDbPath() + bakFileName;
            FileInfo info = new FileInfo(fullname);
            if (info.Exists)
            {
                info.Delete();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取某个文件夹下面指定扩展名的文件
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="extName">扩展名</param>
        /// <returns></returns>
        public static List<FileInfo> GetDirectoryFiles(string path, string extName)
        {
            List<FileInfo> listFile = new List<FileInfo>();
            string[] files = System.IO.Directory.GetFiles(@path, "*." + extName, System.IO.SearchOption.TopDirectoryOnly);//获取该目录下的Doc文件

            foreach (string fileName in files)
            {
                FileInfo fi = new FileInfo(fileName);
                listFile.Add(fi);
            }
            return listFile;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取当前数据库名称
        /// </summary>
        /// <returns></returns>
        public static string GetDbName()
        {
            //return WebTools.GetAppConfig("Database");
            var con = DbHelperSQL.connectionString;
            //从这里面分析出数据库名称
            var flag = "Initial Catalog=";
            var start = con.IndexOf(flag) + flag.Length;
            var end = con.Substring(start).IndexOf(";");
            var name = con.Substring(start, end);
            return name;
        }
        /// <summary>
        /// 获取当前数据的目录
        /// </summary>
        /// <returns></returns>
        private static string GetDbPath()
        {
            string path = HttpContext.Current.Server.MapPath("~/backup/");
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
            return path;
        }
        #endregion
    }
}