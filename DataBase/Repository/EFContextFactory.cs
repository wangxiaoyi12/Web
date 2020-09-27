using Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Management;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace DataBase
{
    public sealed class EFContextFactory
    {
        #region Fields
        /// <summary>
        /// 定时间隔 ms
        /// </summary>
        private static int Interval = 5000;
        /// <summary>
        /// 定时器
        /// </summary>
        static System.Timers.Timer sysTimer = new System.Timers.Timer();
        /// <summary>
        /// 锁定方法用到的对象
        /// </summary>
        private readonly static object lockobj = new object();
        private readonly static object lockobj2 = new object();
        private static int logCount = 0;
        private static int curlogCount = 0;
        /// <summary>
        /// 存放 当前请求 与 数据库上下文 的字典
        /// </summary>
        volatile static Dictionary<HttpContext, DbContext> divDataContext = new Dictionary<HttpContext, DbContext>();
        #endregion

        #region 构造器--定时器  定时清理已结束的请求的上下文
        static EFContextFactory()
        {
            //logCount = 60 * 60 * 1000 / Interval;  //1 小时记录一次
            //sysTimer.AutoReset = true;
            //sysTimer.Interval = Interval;
            //sysTimer.Enabled = true;
            //sysTimer.Elapsed += new System.Timers.ElapsedEventHandler(sysTimer_Elapsed);
            //sysTimer.Start();
        }
        /// <summary>           
        /// 订阅Elapsed事件的方法       
        /// </summary>         
        /// <param name="sender"></param>     
        /// <param name="e"></param>    
        static void sysTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs ex)
        {
            lock (lockobj2)
            {
                try
                {
                    List<HttpContext> list = divDataContext.Keys.Where(item => item.Items.Count == 0).ToList();
                    for (int index = 0; index < list.Count; index++)
                    {
                        if (divDataContext[list[index]] != null)
                        {
                            //使用using或Dispose() 释放
                            using (divDataContext[list[index]])
                            {
                            }
                            divDataContext[list[index]] = null;
                        }
                        divDataContext.Remove(list[index]);
                        list[index] = null;
                    }
                    curlogCount++;
                    if (curlogCount >= logCount)
                    {
                        curlogCount = 0;
                        //记录内存占用情况
                        var msize1 = GC.GetTotalMemory(true) / 1024 / 1024.0;
                        double msize2 = 0;
                        var msize3 = System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024.0;
                        #region 获取可用内存                       
                        var cimobject2 = new ManagementClass("Win32_PerfFormattedData_PerfOS_Memory");
                        ManagementObjectCollection moc2 = cimobject2.GetInstances();
                        foreach (ManagementObject mo2 in moc2)
                        {
                            msize2 += double.Parse(mo2.Properties["AvailableMBytes"].Value.ToString());
                        }
                        moc2.Dispose();
                        cimobject2.Dispose();
                        #endregion
                        LogHelper.Debug(string.Format("内存占用(M)：托管内存：{0} ，可用内存：{1}，进程占用：{2},当前实例数:{3}", msize1, msize2, msize3, divDataContext.Count));
                        //ClearMemory();  //释放内存,不要频繁的释放内存
                    }
                }
                catch (Exception e)
                {
                    LogHelper.Error("定时清理上下文出错：" + WebTools.getFinalException(e));
                }
            }
        }
        #endregion

        #region 将DbContext上下文放到线程中，使线程内唯一，暂时不采用
        //帮我们返回当前线程内的数据库上下文，如果当前线程内没有上下文，那么创建一个上下文，并保证
        //上线问实例在线程内部是唯一的
        private static int DBCount = 0;
        public static DbContext GetCurrentDbContext()
        {
            //CallContext：是线程内部唯一的独用的数据槽（一块内存空间）
            //传递DbContext进去获取实例的信息，在这里进行强制转换。
            DbContext dbContext = CallContext.GetData("DbContext") as DbContext;
            if (dbContext == null)  //线程在数据槽里面没有此上下文
            {
                //DBCount++;
                //LogHelper.SQL("dbcount:" + DBCount);
                dbContext = new MyConfig(); //如果不存在上下文的话，创建一个EF上下文
                //我们在创建一个，放到数据槽中去
                CallContext.SetData("DbContext", dbContext);
            }
            return dbContext;
        }
        #endregion 

        #region 获取当前请求对应的DbContext上下文        
        public static DbContext GetHttpContextDbContext()
        {
            lock (lockobj)
            {
                if (!divDataContext.Keys.Contains(HttpContext.Current))
                {
                    divDataContext.Add(HttpContext.Current, new MyConfig());
                }
                return divDataContext[HttpContext.Current];
            }           
        }
        #endregion

       
    }
}
