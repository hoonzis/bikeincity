using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Caching;
using BikeInCity.Web.Biking;
using System.Xml.Linq;
using BikeInCity.Model;
using System.Globalization;
using System.Configuration;
using BikeInCity.Core.Services;
using Ninject;
using Ninject.Web;
using Ninject.Modules;
using BikeInCity.Web.Technical;
using NHibernate.Context;
using NHibernate;
using Quartz;
using BikeInCity.Web.Tasks;
using Quartz.Impl;
using NHibernate.Linq;
using Common.Logging;

namespace BikeInCity.Web
{
    public class Global : NinjectHttpApplication
    {
        private static IKernel _kernel;
        private static IScheduler _scheduler;
        public static int RepeatInterval { get; set; }
        private readonly ILog _log = LogManager.GetLogger(typeof(Global));


        public static IScheduler Scheduler
        {
            get { return _scheduler; }
            set { _scheduler = value; }
        }


        private static Dictionary<String, IBikeCity> _cities = new Dictionary<String, IBikeCity>();

        public static Dictionary<String, IBikeCity> Cities
        {
            get { return Global._cities; }
            set { Global._cities = value; }
        }
        
        [Inject]
        public ICityService CityService { get; set; }

        static private ISessionFactory _sessionFactory;
        public static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    SessionFactoryProvider.CreateSessionFactoryInstance();
                    _sessionFactory = SessionFactoryProvider.SessionFactory;
                }
                return _sessionFactory;
            }
        }

        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();
            _log.Info("Application started! Cities will be added to the scheduler");
            AddCitiesToScheduler();
        }

        public static T GetObject<T>()
        {
            return _kernel.Get<T>();
        }

        private void AddCitiesToScheduler()
        {
             RepeatInterval = Convert.ToInt32(ConfigurationManager.AppSettings["RepeatInterval"]);
            _cities.Add("Rennes",new Rennes());
            _cities.Add("Wien",new Wien());
            _cities.Add("London",new London());
            _cities.Add("Denver",new Denver());
            _cities.Add("Paris",new Paris());
            _cities.Add("Valencia",new Valencia());
            _cities.Add("Prague",new Prague());
            _cities.Add("Lyon",new Lyon());
            _cities.Add("Marseille",new Marseille());
            _cities.Add("Toulouse",new Toulouse());
            _cities.Add("Montreal",new Montreal());
            _cities.Add("Caen",new Caen());
            _cities.Add("Dijon",new Dijon());
            _cities.Add("Perpigan",new Perpignan());
            _cities.Add("NextBike",new NextBike());


            CreateScheduler();    

            foreach (var city in _cities)
            {
                AddTask(city.Key);
            }

                
        }

        public static IBikeCity GetBikeCity(String name)
        {
            return _cities[name];
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            CurrentSessionContext.Bind(SessionFactory.OpenSession());
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            ISession session = CurrentSessionContext.Unbind(SessionFactory);

            if (session != null)
            {
                session.Close();
            }
        }

        protected override IKernel CreateKernel()
        {
            var module = new BICWebModule();
            IKernel kernel = new StandardKernel(module);
            _kernel = kernel;
            return kernel;
        }

        private void CreateScheduler()
        {
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            _scheduler = schedFact.GetScheduler();
        }

        public static Dictionary<String, String> CityStatuses = new Dictionary<string, string>();

        private void AddTask(String cityName)
        {
            // construct job info
            IJobDetail jobDetail = JobBuilder
                .Create()
                .OfType(typeof(BikeTask))
                .WithIdentity(new JobKey(cityName,"citiesJobs"))
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .WithIdentity(new TriggerKey(cityName, "citiesTrigger"))
                .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInSeconds(60))
                .StartAt(DateTime.UtcNow.AddSeconds(1))
                .Build();

            _scheduler.ScheduleJob(jobDetail, trigger); 
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Redirect mobile users to the mobile home page
            HttpRequest httpRequest = HttpContext.Current.Request;
            if (httpRequest.Browser.IsMobileDevice)
            {
                string path = httpRequest.Url.PathAndQuery;
                bool isOnMobilePage = path.StartsWith("/Mobile/",
                                       StringComparison.OrdinalIgnoreCase);
                if (!isOnMobilePage)
                {
                    string redirectTo = "~/Mobile/";

                    // Could also add special logic to redirect from certain 
                    // recognized pages to the mobile equivalents of those 
                    // pages (where they exist). For example,
                    // if (HttpContext.Current.Handler is UserRegistration)
                    //     redirectTo = "~/Mobile/Register.aspx";

                    HttpContext.Current.Response.Redirect(redirectTo);
                }
            }
        }
    }
}