using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ninject;
using BikeInCity.Core.Services;
using Ninject.Web;
using BikeInCity.Web.Technical;
using BikeInCity.DataAccess.Configuration;
using BikeInCity.Core.DataAccess;
using BikeInCity.Model;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using Common.Logging;
using BikeInCity.Dto;
using AutoMapper;

namespace BikeInCity.Web.Pages
{
    public partial class Default : PageBase
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Default));

        [Inject]
        public ICityService CityService { get; set; }

        [Inject]
        public IRepository Repository { get; set; }
        
        public Default()
        {
            LoadComplete += new EventHandler(Admin_LoadComplete);
        }

        void Admin_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                var stationCount = Repository.GetAll<Station>().Count();
                var citiesCount = Repository.GetAll<City>().Count();

                lblOutput.Text = "Cities count: " + citiesCount + " Stations Count: " + stationCount + " Repeat interval: " + Global.RepeatInterval;
                lblSchedulerState.Text = Global.Scheduler.IsStarted ? Global.Scheduler.InStandbyMode ? "StandBy" : "Started" : "Stopped";

                StringBuilder builder = new StringBuilder();
                foreach (var cityStatus in Global.CityStatuses)
                {
                    builder.Append(cityStatus.Key + " - " + ((cityStatus.Value.Length > 30) ? cityStatus.Value.Substring(0, 30) : cityStatus.Value + Environment.NewLine));
                }
                lblCityStatuses.Text = builder.ToString();
            }
            catch (SqlException ex)
            {
                lblOutput.Text = "The database was not yet configured! + \n" + ex.Message;
            }
            catch (Exception ex)
            {
                lblOutput.Text = "Other unspecified execption! + \n" + ex.Message;
            }
        }

        public void Reinsert_Click(object sender, EventArgs e)
        {
            var citiesXML = WebUtils.GetAppDataPath("cityList.xml");
            CityService.ReinsertCitiesToDB("http://nextbike.net/maps/nextbike-official.xml", citiesXML);
            lblOutput.Text = "Cities inserted";
        }

        public void RegenerateDB_Click(object sender, EventArgs e)
        {
            Utils.RegenerateDatabaseSchema();
        }

        public void DownloadAllCities_Click(object sender, EventArgs e)
        {
            foreach (var cityCompany in Global.Cities)
            {
                try
                {
                    var cities = cityCompany.Value.ProcessCity();
                    foreach (var city in cities)
                    {
                        CityService.UpdateCityStations(city);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString() + "in City : " + cityCompany.Key);
                }
            }
        }

        public void SchedulerStart_Click(object sender, EventArgs e)
        {
            Global.Scheduler.Start();
            _log.Info("Scheduler started");
        }

        public void SchedulerStop_Click(object sender, EventArgs e)
        {
            Global.Scheduler.Standby();
            _log.Info("Scheduler on Standby");
        }

        private void SerializeAll<T>(List<T> items)
        {
            var serializer = new DataContractJsonSerializer(typeof(List<T>));
            var fileName = WebUtils.GetAppDataPath(typeof(T).Name + DateTime.Now.ToString("ddMMyyyy"));
            using (var stream = File.Create(fileName)) {
                serializer.WriteObject(stream, items);
            }
        }

        public void BackupToJson_Click(object sender, EventArgs e)
        {
            var tips = Mapper.Map<List<InformationTipDto>>(Repository.GetAll<InformationTip>());
            SerializeAll<InformationTipDto>(tips);

            var cities = Mapper.Map<List<CityDto>>(Repository.GetAll<City>());
            SerializeAll<CityDto>(cities);

            var stations = Mapper.Map<List<StationDto>>(Repository.GetAll<Station>());
            SerializeAll<StationDto>(stations);

            var countries = Mapper.Map<List<CountryDto>>(Repository.GetAll<Country>());
            SerializeAll<CountryDto>(countries);

            lblOutput.Text = "Backup OK";
        }

        public void RemoveAllStations_Click(object sender, EventArgs e)
        {
            Repository.ExecuteUpdateQuery("delete Station s");
        }
    }
}