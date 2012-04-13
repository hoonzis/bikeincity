using NHibernate;

namespace  BikeInCity.DataAccess.Configuration
{
    public interface ISessionFactoryFactory
    {
        ISessionFactory GetSessionFactory();
    }
}
