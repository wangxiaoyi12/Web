using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ThoughtWorks.QRCode.Codec;

namespace Common
{
    public static class ImageCombin
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recUrl"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static string CombinQR(string recUrl, string Id, string HeadUrl)
        {
            var context = HttpContext.Current;
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 4;
            qrCodeEncoder.QRCodeVersion = 8;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            //二维码
            System.Drawing.Image image = qrCodeEncoder.Encode(recUrl);
            //设置大小
            image = KiResizeImage(image, 630, 630, 0);
            var imgQRUrl = context.Server.MapPath("/Upload/QRImage/qr" + DateTime.Now.ToString("mmssfff") + ".jpg");
            image.Save(imgQRUrl);

            System.Drawing.Image imgBackup = Image.FromFile(context.Server.MapPath("/assets/mobile/img/template.jpg"));

            //System.IO.MemoryStream MStream = new System.IO.MemoryStream();
            //image.Save(MStream, System.Drawing.Imaging.ImageFormat.Png);

            //System.IO.MemoryStream MSFinish = new System.IO.MemoryStream();
            var img = CombinImage(imgBackup, imgQRUrl, 630, 630, 225, 890);
            var imgUrl = "/Upload/QRImage/" + DateTime.Now.ToString("mmssfff") + ".jpg";
            img.Save(context.Server.MapPath(imgUrl));

            imgBackup = img;

            var imgNew = CombinImage(imgBackup, context.Server.MapPath(HeadUrl), 200, 200, 440, 150);
            var imgNewUrl = "/Upload/QRImage/QR_" + DateTime.Now.ToString("mmssfff") + ".jpg";
            imgNew.Save(context.Server.MapPath(imgNewUrl));
            image.Dispose();
            img.Dispose();

            //MStream.Dispose();
            //MSFinish.Dispose();
            return imgNewUrl;
        }


        /// <summary>  
        /// 调用此函数后使此两种图片合并，类似相册，有个  
        /// 背景图，中间贴自己的目标图片  
        /// </summary>  
        /// <param name="imgBack">粘贴的源图片,背景图</param>  
        /// <param name="destImg">上层图片</param>  
        public static Image CombinImage(Image imgBack, string destImg, int H, int W, int X, int Y)
        {
            Image img = Image.FromFile(destImg);        //照片图片    
            if (img.Height != H || img.Width != W)
            {
                img = KiResizeImage(img, W, H, 0);
            }
            Graphics g = Graphics.FromImage(imgBack);

            g.DrawImage(imgBack, 0, 0, imgBack.Width, imgBack.Height);      //g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);   

            // g.FillRectangle(System.Drawing.Brushes.White, imgBack.Width / 2 - img.Width / 2 - 1, imgBack.Width / 2 - img.Width / 2 - 1, 1, 1);//相片四周刷一层黑色边框  

            //g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);  

            //g.DrawImage(img, imgBack.Width / 2 - img.Width / 2, imgBack.Width / 2 - img.Width / 2, img.Width, img.Height);
            g.DrawImage(img, X, Y, img.Width, img.Height);
            GC.Collect();
            return imgBack;
        }


        /// <summary>  
        /// Resize图片  
        /// </summary>  
        /// <param name="bmp">原始Bitmap</param>  
        /// <param name="newW">新的宽度</param>  
        /// <param name="newH">新的高度</param>  
        /// <param name="Mode">保留着，暂时未用</param>  
        /// <returns>处理以后的图片</returns>  
        public static Image KiResizeImage(Image bmp, int newW, int newH, int Mode)
        {
            try
            {
                Image b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量  
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }
    }
}
