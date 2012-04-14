using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Activation;
using FluentNHibernate.Cfg;
using BikeInCity.DataAccess.Configuration;
using NHibernate.Tool.hbm2ddl;
using NHibernate;

namespace BikeInCity.Web.Technical
{
    public class SessionFactoryProvider : IProvider
    {
        
        public static ISessionFactory SessionFactory;
        private static bool _rebuildSchema;

        public static void CreateSessionFactoryInstance()
        {
            var config = new NHibernate.Cfg.Configuration();

            SessionFactory = Fluently.Configure(config)
                .Mappings(ConfigureMapping)
                .ExposeConfiguration(NHibernateStuff)
                .BuildSessionFactory();

            _rebuildSchema = true;
            
        }

        private static void ConfigureMapping(MappingConfiguration config)
        {
            config.FluentMappings
                //.Conventions.AddFromAssemblyOf<IndexNameConvention>()
                .AddFromAssemblyOf<SessionFactoryFactory>();
        }
        private static void NHibernateStuff(NHibernate.Cfg.Configuration config)
        {
            if (_rebuildSchema)
                new SchemaExport(config).Create(_rebuildSchema, true);
        }

        public object Create(IContext context)
        {
            if (SessionFactory == null)
            {
                CreateSessionFactoryInstance();
            }
            return SessionFactory; 
        }

        public Type Type
        {
            get { return typeof(ISessionFactory); }
        }
    }
}