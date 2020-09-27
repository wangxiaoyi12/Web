using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 分页
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageList<T> : BaseList<T>
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 记录条数
        /// </summary> 
        public int RecordCount { get; set; }        
        /// <summary>
        /// 空构造器
        /// </summary>
        public PageList()
        {
        }
        /// <summary>
        /// 无分页构造器
        /// </summary>
        /// <param name="code">Code为1时成功，其他失败</param>
        /// <param name="list">数据列表</param>
        public PageList(List<T> list)
        {
            if (list == null)
            {
                this.Code = 0;
            }
            else
            {
                this.Code = 1;
            }
            this.List = list;
            this.PageIndex = 1;
            this.PageSize = int.MaxValue;
            this.RecordCount = list.Count;
        }
        /// <summary>
        /// 无分页构造器
        /// </summary>
        /// <param name="code">Code为1时成功，其他失败</param>
        /// <param name="list">数据列表</param>
        public PageList(int code, List<T> list)
        {
            this.Code = code;
            this.List = list;
            this.PageIndex = 1;
            this.PageSize = int.MaxValue;
            this.RecordCount = list.Count;
        }
        /// <summary>
        /// 分页构造器
        /// </summary>
        /// <param name="list">数据列表</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="recordCount">记录总行数</param>
        public PageList(List<T> list, int pageIndex, int pageSize, int recordCount)
        {
            if (list == null)
            {
                this.Code = 0;
            }
            else
            {
                this.Code = 1;
            }
            this.List = list;
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.RecordCount = recordCount;
        }
        /// <summary>
        /// 分页构造器
        /// </summary>
        /// <param name="code">Code为1时成功，其他失败</param>
        /// <param name="list">数据列表</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="recordCount">记录总行数</param>
        public PageList(int code, List<T> list, int pageIndex, int pageSize, int recordCount)
        {
            this.Code = code;
            this.List = list;
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.RecordCount = recordCount;
        }
        /// <summary>
        /// 分页构造器
        /// </summary>     
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页大小</param>
        public PageList(int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
        }
    }

    /// <summary>
    /// 分页
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="M"></typeparam>
    public class PageList<T, M> : PageList<T>
    {
        /// <summary>
        /// 累计值
        /// </summary>
        public M Sum { get; set; }

        /// <summary>
        /// 分页构造器
        /// </summary>     
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页大小</param>
        public PageList(int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
        }
    }
}
