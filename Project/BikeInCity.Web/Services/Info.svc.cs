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
using Common.Logging;
using System.Net;
using AutoMapper;

namespace BikeInCity.Web.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceContract]
    public class Info
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(Info));

        private IRepository _repository;
        private IInfoService _infoService;
        
        public Info()
        {
            _repository = Global.GetObject<IRepository>();
            _infoService = Global.GetObject<IInfoService>();
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
                var list = Mapper.Map<List<InformationTipDto>>(_repository.Find<InformationTip>(x => x.City.Id == id));
                return list;
            }
            catch (FormatException ex)
            {
                _log.Info("WS exception", ex);
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }
        }
    }
}
