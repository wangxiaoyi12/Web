/*
 * 文件名称：Thumbnail.cs
 * 摘    要：存储上传图片数据的缓存对象，存储原图和缩略图。
 * 这个例程是根据官网下载的示例改的。增加了保存和删除的功能。
 * 原始例子可以参看示例中applicationdemo.net工程。
 * 
 * 当前版本：1.0
 * 作    者：虞晓杰
 * 完成日期：2012月8月9日
 */

namespace Common
{
    /// <summary>
    /// 存储图片缓存数据
    /// </summary>
    public class Thumbnail
    {
        #region 类内部变量
        private string m_Id;
        private byte[] m_ThumbnailData;
        private byte[] m_OriginalData;
        #endregion

        #region 类属性
        /// <summary>
        /// 图片的ID
        /// </summary>
        public string ID
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        /// <summary>
        /// 缩略图数据
        /// </summary>
        public byte[] ThumbnailData
        {
            get { return m_ThumbnailData; }
            set { m_ThumbnailData = value; }
        }
        /// <summary>
        /// 原始数据
        /// </summary>
        public byte[] OriginalData
        {
            get { return m_OriginalData; }
            set { m_OriginalData = value; }
        }
        #endregion

        #region 类初始化
        public Thumbnail(string id, byte[] thudata, byte[] oridata)
        {
            this.ID = id;
            this.ThumbnailData = thudata;//缩略图信息流
            this.OriginalData = oridata;//原图信息流
        }
        #endregion
    }
}