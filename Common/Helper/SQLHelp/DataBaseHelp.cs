/**********************************************************************************
 *
 * 功能说明:备份和恢复SQL Server数据库
 * 作者: 刘功勋;
 * 版本:V0.1(C#2.0);时间:2007-1-1
 * 当使用SQL Server时,请引用 COM组件中的,SQLDMO.dll组件
 * 当使用Access中,请浏览添加引用以下两个dll
 *          引用C:\Program Files\Common Files\System\ado\msadox.dll,该DLL包含ADOX命名空间
 *          引用C:\Program Files\Common Files\System\ado\msjro.dll,该DLL包含JRO命名空间
 * *******************************************************************************/
using System;

namespace Common
{
    /// <summary>
    /// 数据库恢复和备份
    /// </summary>
    public class DataBaseHelp
    {
        public DataBaseHelp()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        #region SQL数据库备份
        /// <summary>
        /// SQL数据库备份
        /// </summary>
        /// <param name="ServerIP">SQL服务器IP或(Localhost)</param>
        /// <param name="LoginName">数据库登录名</param>
        /// <param name="LoginPass">数据库登录密码</param>
        /// <param name="DBName">数据库名</param>
        /// <param name="BackPath">备份到的路径</param>
        public static void SQLBACK(string ServerIP, string LoginName, string LoginPass, string DBName, string BackPath)
        {
            SQLDMO.Backup oBackup = new SQLDMO.BackupClass();
            SQLDMO.SQLServer oSQLServer = new SQLDMO.SQLServerClass();
            try
            {
                oSQLServer.LoginSecure = false;
                oSQLServer.Connect(ServerIP, LoginName, LoginPass);
                oBackup.Database = DBName;
                oBackup.Files = BackPath;
                oBackup.BackupSetName = DBName;
                oBackup.BackupSetDescription = "数据库手工备份";
                oBackup.Initialize = true;
                oBackup.SQLBackup(oSQLServer);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                oSQLServer.DisConnect();
            }
        }
        #endregion
        #region SQL恢复数据库
        /// <summary>
        /// SQL恢复数据库
        /// </summary>
        /// <param name="ServerIP">SQL服务器IP或(Localhost)</param>
        /// <param name="LoginName">数据库登录名</param>
        /// <param name="LoginPass">数据库登录密码</param>
        /// <param name="DBName">要还原的数据库名</param>
        /// <param name="BackPath">数据库备份的路径</param>
        public static void SQLDbRestore(string ServerIP, string LoginName, string LoginPass, string DBName, string BackPath)
        {

            SQLDMO.Restore orestore = new SQLDMO.RestoreClass();
            SQLDMO.SQLServer oSQLServer = new SQLDMO.SQLServerClass();
            try
            {
                KillProcesses(DBName); //杀死此数据库的进程
                oSQLServer.LoginSecure = false;
                oSQLServer.Connect(ServerIP, LoginName, LoginPass);
                orestore.Action = SQLDMO.SQLDMO_RESTORE_TYPE.SQLDMORestore_Database;
                orestore.Database = DBName;
                orestore.Files = BackPath;
                orestore.FileNumber = 1;
                orestore.ReplaceDatabase = true;
                orestore.SQLRestore(oSQLServer);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                oSQLServer.DisConnect();
            }
        }

        #endregion

        #region  获得SQL实例
        /// <summary>
        /// 获得SQL实例
        /// </summary>
        /// <returns></returns>
        private static SQLDMO.SQLServer getSQL()
        {
            var Host = WebTools.GetAppConfig("HostName");
            var UID = WebTools.GetAppConfig("UID");
            var Pwd = WebTools.GetAppConfig("Pwd");

            SQLDMO.SQLServer sqlserver = new SQLDMO.SQLServerClass();
            sqlserver.LoginSecure = false;
            sqlserver.Connect(Host, UID, Pwd);
            return sqlserver;
        }
        #endregion
        #region 杀死此数据库的进程

        /// <summary>
        /// 杀死此数据库的进程
        /// </summary>
        /// <param name="DatabaseName">数据库名称</param>
        private static void KillProcesses(string DatabaseName)
        {
            SQLDMO.SQLServer oSQLServer = getSQL(); ;
            SQLDMO.QueryResults qr = oSQLServer.EnumProcesses(-1);

            int iColPIDNum = -1;
            int iColDbName = -1;

            //杀死其它的连接进程  
            for (int i = 1; i <= qr.Columns; i++)
            {
                string strName = qr.get_ColumnName(i);

                if (strName.ToUpper().Trim() == "SPID")
                {
                    iColPIDNum = i;
                }
                else if (strName.ToUpper().Trim() == "DBNAME")
                {
                    iColDbName = i;
                }
                if (iColPIDNum != -1 && iColDbName != -1)
                    break;
            }

            for (int i = 1; i <= qr.Rows; i++)
            {
                int lPID = qr.GetColumnLong(i, iColPIDNum);
                string strDBName = qr.GetColumnString(i, iColDbName);
                if (strDBName.ToUpper() == DatabaseName.ToUpper())
                    oSQLServer.KillProcess(lPID);
            }
        }
        #endregion
    }
}