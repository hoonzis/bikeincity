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

namespace BikeInCity.Web.Pages
{
    public partial class Admin : PageBase
    {
        [Inject]
        public ICityService CityService { get; set; }

        [Inject]
        public IRepository Repository { get; set; }
        
        public Admin()
        {
            LoadComplete += new EventHandler(Admin_LoadComplete);
        }

        void Admin_LoadComplete(object sender, EventArgs e)
        {
            lblSchedulerState.Text = Global.Scheduler.IsStarted ? "Started" : "Stopped";
            StringBuilder builder = new StringBuilder();
            foreach (var cityStatus in Global.CityStatuses)
            {
                builder.Append(cityStatus.Key + " - " + ((cityStatus.Value.Length > 30) ? cityStatus.Value.Substring(0, 30) : cityStatus.Value + Environment.NewLine));
            }
            lblCityStatuses.Text = builder.ToString();
        }

        public void Reinsert_Click(object sender, EventArgs e)
        {
            var citiesXML = WebUtils.GetAppDataPath("cityList.xml");
            Logger.WriteMessage(citiesXML);
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
        }

        public void SchedulerStop_Click(object sender, EventArgs e)
        {
            Global.Scheduler.Shutdown();
        }

        public void BackupToJson_Click(object sender, EventArgs e)
        {
            var tips = Repository.GetAll<InformationTip>();
            var serializer = new DataContractJsonSerializer(typeof(List<InformationTip>));
            var fileName = WebUtils.GetAppDataPath("backup" + DateTime.Now.ToString("ddMMyyyy"));
            var stream = File.Create(fileName);
            serializer.WriteObject(stream, tips);
            lblOutput.Text = "Backup OK";
        }
    }
}