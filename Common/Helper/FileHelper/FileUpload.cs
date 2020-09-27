using System;
using System.Web;
using System.IO;

namespace Common
{
    /// <summary>
    /// 文件、图片 上传通用类
    /// add dukang by 20160624
    /// </summary>
    public class FileUpload
    {

        public static string UpLoad(string path, HttpRequestBase request, HttpServerUtilityBase server, int urltype)
        {
            //上传和返回(保存到数据库中)的路径
            var tempPath = server.MapPath(path);
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);//不存在就创建目录 
            }
            if (request.Files.Count <= 0) return string.Empty;
            var imgFile = request.Files["file"];
            if (imgFile == null) return "";
            //创建图片新的名称
            var nameImg = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            //获得上传图片的路径
            var strPath = imgFile.FileName;
            //获得上传图片的类型(后缀名)
            var type = strPath.Substring(strPath.LastIndexOf(".", StringComparison.Ordinal) + 1).ToLower();

            //拼写数据库保存的相对路径字符串
            // savepath = "..\\" + path + "\\";
            path += nameImg + "." + type;
            //拼写上传图片的路径
            var uppath = server.MapPath(path);
            // uppath += nameImg + "." + type;
            //上传图片
            imgFile.SaveAs(uppath);
            switch (urltype)
            {
                case 1:
                    return path;
                case 2:
                    return uppath;
            }
            return string.Empty;
        }
        public static bool ValidateImg(string imgName)
        {
            string[] imgType = new string[] { "gif", "jpg", "png", "bmp" };

            int i = 0;
            bool blean = false;
            string message = string.Empty;

            //判断是否为Image类型文件
            while (i < imgType.Length)
            {
                if (imgName.Equals(imgType[i].ToString()))
                {
                    blean = true;
                    break;
                }
                else if (i == (imgType.Length - 1))
                {
                    break;
                }
                else
                {
                    i++;
                }
            }
            return blean;
        }

    }
}
