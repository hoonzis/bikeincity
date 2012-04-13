using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace BikeInCity.Web.Services
{
    /// <summary>
    /// Summary description for Upload
    /// </summary>
    public class Upload : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            System.Web.HttpPostedFile file = context.Request.Files[0];

            //Check extension
            string extension = file.FileName.Split('.')[file.FileName.Split('.').Length - 1];

            if (extension != "jpg" && extension!= "png" && extension!= "PNG")
            {
                context.Response.StatusCode = 400;
                context.Response.Write("{\"error\",\"Error with the extension of the file\"");
                return;
            }

            var fileName = RandomString(20) + "." + extension;
            var folderName = "Uploads";
            
            var filePath = System.Web.HttpContext.Current.Server.MapPath("~") + folderName + "\\" + fileName;
            var link = String.Format("/{0}/{1}", folderName, fileName);
            file.SaveAs(filePath);
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

        private static Random random = new Random((int)DateTime.Now.Ticks);//thanks to McAden
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