using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BikeInCity.Web.Services
{
    /// <summary>
    /// Summary description for Upload
    /// </summary>
    public class Upload : IHttpHandler
    {

        private static Random random = new Random((int)DateTime.Now.Ticks);//thanks to McAden

        public void ProcessRequest(HttpContext context)
        {
            System.Web.HttpPostedFile file = context.Request.Files[0];
            
            
            //Check extension
            String[] byPoints = file.FileName.Split('.');
            if (byPoints.Length <2)
            {
                context.Response.StatusCode = 400;
                context.Response.Write("{\"error\",\"Error with the extension of the file\"");
                return;
            }

            string extension = byPoints[byPoints.Length - 1].ToLowerInvariant();
            
            //accepting only jpg and png
            if (extension != "jpg" && extension!= "png")
            {
                context.Response.StatusCode = 400;
                context.Response.Write("{\"error\",\"Error with the extension of the file\"");
                return;
            }

            var fileName = RandomString(20) + "." + extension;
            var folderName = "Uploads";
            var filePath = System.Web.HttpContext.Current.Server.MapPath("~") + folderName + "\\" + fileName;
            var link = String.Format("/{0}/{1}", folderName, fileName);

            using (var img = Image.FromStream(file.InputStream, true, false ) )
            {
	            Size sz = adaptProportionalSize(new Size(250,250), img.Size);
	            var smallImage = img.GetThumbnailImage(sz.Width,sz.Height,null,IntPtr.Zero );
                smallImage.Save(filePath);
            }
            context.Response.StatusCode = 200;
            context.Response.Write(link);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Thanks to: http://danbystrom.se/2009/01/05/imagegetthumbnailimage-and-beyond/
        /// </summary>
        /// <param name="szMax"></param>
        /// <param name="szReal"></param>
        /// <returns></returns>
        private static Size adaptProportionalSize(Size szMax,Size szReal)
        {
            int nWidth;
            int nHeight;
            double sMaxRatio;
            double sRealRatio;

            if (szMax.Width < 1 || szMax.Height < 1 || szReal.Width < 1 || szReal.Height < 1)
                return Size.Empty;

            sMaxRatio = (double)szMax.Width / (double)szMax.Height;
            sRealRatio = (double)szReal.Width / (double)szReal.Height;

            if (sMaxRatio < sRealRatio)
            {
                nWidth = Math.Min(szMax.Width, szReal.Width);
                nHeight = (int)Math.Round(nWidth / sRealRatio);
            }
            else
            {
                nHeight = Math.Min(szMax.Height, szReal.Height);
                nWidth = (int)Math.Round(nHeight * sRealRatio);
            }

            return new Size(nWidth, nHeight);
        }


        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}