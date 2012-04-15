using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BikeInCity.Model;
using System.ServiceModel.Web;
using BikeInCity.Web.Technical;
using BikeInCity.Core.Services;
using BikeInCity.Dto;
using System.ServiceModel.Activation;
using System.Web;
using System.Configuration;
using BikeInCity.Core.DataAccess;
using log4net;
using System.Net;

namespace BikeInCity.Web.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceContract]
    public class Info
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(Info));

        private IRepository _repository;
        private IInfoService _infoService;
        private IImageService _imageService;

        public Info()
        {
            _repository = Global.GetObject<IRepository>();
            _infoService = Global.GetObject<IInfoService>();
            _imageService = Global.GetObject<IImageService>();
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json, UriTemplate = "tips/add")]
        public void AddTip(InformationTipDto tip)
        {
            int id = Convert.ToInt32(tip.CityId);
            _infoService.AddInformationTip(tip.Title, tip.Description, tip.ImageUrl, id, tip.Lat, tip.Lng);
        }

        [OperationContract]
        [WebGet(UriTemplate = "city/{cityId}/tips", ResponseFormat = WebMessageFormat.Json)]
        public List<InformationTipDto> InformationTips(String cityId)
        {
            try
            {
                var id = Convert.ToInt32(cityId);
                var list = _repository.Find<InformationTip>(x => x.City.Id == id).Select(x=>Mapper.Map(x)).ToList();
                return list;
            }
            catch (FormatException ex)
            {
                _log.Info("WS exception", ex);
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "images/add")]
        public bool UploadImage(ImageDto image)
        {
            var filePath = HttpContext.Current.Server.MapPath(".") +
                       ConfigurationManager.AppSettings["PictureUploadDirectory"];

            return _imageService.UploadImage(image.Data, image.Title, filePath);
        }
    }
}
