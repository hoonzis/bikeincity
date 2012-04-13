using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BikeInCity.Core.Services
{
    public interface IImageService
    {
        bool UploadImage(byte[] data, String title,String filePath);
    }
}
