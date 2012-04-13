using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate.Cfg;

namespace BikeInCity.DataAccess.Configuration
{
    public static class Utils
    {
        private static void ConfigureMapping(MappingConfiguration config)
        {
            config.FluentMappings
                //.Conventions.AddFromAssemblyOf<IndexNameConvention>()
                .AddFromAssemblyOf<SessionFactoryFactory>();
        }

        public static void RegenerateDatabaseSchema()
        {
            var config = new NHibernate.Cfg.Configuration();

            Fluently.Configure(config)
                .Mappings(ConfigureMapping)
                .ExposeConfiguration(NHibernateStuff)
                .BuildSessionFactory();
  
        }

        private static void NHibernateStuff(NHibernate.Cfg.Configuration config)
        {
            new SchemaExport(config).Create(true, true);
        }
    }
}
