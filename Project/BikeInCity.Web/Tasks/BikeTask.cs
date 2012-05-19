using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using System.Collections;
using BikeInCity.Web.Biking;
using NHibernate;
using NHibernate.Linq;
using Ninject;
using BikeInCity.Model;
using BikeInCity.Core.Services;
using Common.Logging;


namespace BikeInCity.Web.Tasks
{
    public class BikeTask : IJob
    {
        private ISessionFactory _sessionFactory;

        private readonly ILog _log = LogManager.GetLogger(typeof(BikeTask));

        public ISessionFactory SessionFactory
        {
            get {
                if (_sessionFactory == null)
                {
                    _sessionFactory = Global.GetObject<ISessionFactory>();
                }
                return _sessionFactory; }
            set { _sessionFactory = value; }
        }

        
        public void Execute(IJobExecutionContext context)
        {
            
            string instName = context.JobDetail.Key.Name;
            string instGroup = context.JobDetail.Key.Group;

            if (!Global.CityStatuses.ContainsKey(instName))
            {
                Global.CityStatuses.Add(instName, "Not yet executed");
            }
            try
            {
                var dataMap = context.JobDetail.JobDataMap;
                var session = SessionFactory.OpenSession();
                //use one session per the list of cities


                IBikeCity bikeCity = Global.GetBikeCity(instName);
                List<City> cities = bikeCity.ProcessCity();


                foreach (var city in cities)
                {
                    UpdateCityStations(city, session);
                    session.Close();
                }

                Global.CityStatuses[instName] = "OK";

            }
            catch (Exception ex)
            {
                _log.Error("Error while downloading the cities in the task: " + instName,ex);
                Global.CityStatuses[instName] = ex.Message;
            }
            
        }

        private void UpdateCityStations(City city, ISession session)
        {

            City cityInDB;
            if (city.Id != 0)
            {
                cityInDB = session.Load<City>(city.Id);
            }
            else
            {
                cityInDB = session.Query<City>().Where(x => x.Name == city.Name).FirstOrDefault();
            }

            if (cityInDB == null)
            {
                _log.Error("City with ID: " + city.Id + " or name " + city.Name + " not found in DB");
                return;
            }

            var query = session.CreateQuery("delete Station where CityId = " + cityInDB.Id);
            var result = query.ExecuteUpdate();
            Console.Write(result);

            session.Refresh(cityInDB);
            
            foreach (var station in city.Stations)
            {
                station.City = cityInDB;
                session.Save(station);
            }

            cityInDB.Stations = city.Stations;
            session.Update(cityInDB);
            session.Flush();
        }
    }
}