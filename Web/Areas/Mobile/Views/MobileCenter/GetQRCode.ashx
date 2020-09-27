<%@ WebHandler Language="C#" Class="GetQRCode" %>

using System;
using System.Web;

using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using ThoughtWorks.QRCode.Codec.Util;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

public class GetQRCode : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {

        String data = context.Request["c"];
        if (!string.IsNullOrEmpty(data))
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 4;
            qrCodeEncoder.QRCodeVersion = 8;            
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            System.Drawing.Image image = qrCodeEncoder.Encode(data);
            //设置大小
            image = KiResizeImage(image, 400, 400, 0);
            
            System.IO.MemoryStream MStream = new System.IO.MemoryStream();
            image.Save(MStream, System.Drawing.Imaging.ImageFormat.Png);

            System.IO.MemoryStream MSFinish = new System.IO.MemoryStream();
            CombinImage(image, context.Server.MapPath("assets/mobile/img/template.jpg")).Save(MSFinish, System.Drawing.Imaging.ImageFormat.Png);
            context.Response.ClearContent();
            context.Response.ContentType = "image/png";
            context.Response.BinaryWrite(MSFinish.ToArray());

            image.Dispose();
            MStream.Dispose();
            MSFinish.Dispose();
        }

        context.Response.Flush();
        context.Response.End();
    }

    /// <summary>  
    /// 调用此函数后使此两种图片合并，类似相册，有个  
    /// 背景图，中间贴自己的目标图片  
    /// </summary>  
    /// <param name="imgBack">粘贴的源图片</param>  
    /// <param name="destImg">粘贴的目标图片</param>  
    public static Image CombinImage(Image imgBack, string destImg)
    {
        Image img = Image.FromFile(destImg);        //照片图片    
        if (img.Height != 65 || img.Width != 65)
        {
            img = KiResizeImage(img, 65, 65, 0);
        }
        Graphics g = Graphics.FromImage(imgBack);

        g.DrawImage(imgBack, 0, 0, imgBack.Width, imgBack.Height);      //g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);   

        g.FillRectangle(System.Drawing.Brushes.White, imgBack.Width / 2 - img.Width / 2 - 1, imgBack.Width / 2 - img.Width / 2 - 1,1,1);//相片四周刷一层黑色边框  

        //g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);  

        g.DrawImage(img, imgBack.Width / 2 - img.Width / 2, imgBack.Width / 2 - img.Width / 2, img.Width, img.Height);
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

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}