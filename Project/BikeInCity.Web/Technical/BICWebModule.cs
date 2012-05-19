using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Modules;
using BikeInCity.Core.Services;
using BikeInCity.Services;
using BikeInCity.Core.DataAccess;
using BikeInCity.DataAccess.Repositories;
using NHibernate;

namespace BikeInCity.Web.Technical
{
    public class BICWebModule : NinjectModule
    {

        public override void Load()
        {
            Bind<ICityService>().To<CityService>();
            Bind<IRepository>().To<Repository>();
            Bind<ISessionFactory>().ToConstant(Global.SessionFactory);
            Bind<IInfoService>().To<InfoService>();
        }
    }
}