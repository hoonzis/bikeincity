using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeInCity.Core.Services;
using System.IO;
using System.Web;
namespace BikeInCity.Services
{
    public class ImageService : IImageService
    {
        public bool UploadImage(byte[] data, String title,String filePath)
        {
            FileStream fileStream = null;
            BinaryWriter writer = null;
            
            try
            {
                if (title != string.Empty)
                {
                    fileStream = File.Open(filePath, FileMode.Create);
                    writer = new BinaryWriter(fileStream);
                    writer.Write(data);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
                if (writer != null)
                    writer.Close();
            }
        }
    }
}
