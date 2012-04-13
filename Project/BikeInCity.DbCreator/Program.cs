using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using BikeInCity.DbCreator.Technical;
using System.Configuration;
using NHibernate.Context;
using NHibernate;
using BikeInCity.DataAccess.Configuration;


namespace BikeInCity.DbCreator
{
    class Program
    {
        #region Consts
        private const String SCRIPTS_DIR = "./Scripts";
        private const String DB_CREATION_SCRIPT_FILENAME = "Database_Creation.sql";
        private const String USER_NAME = "BikeInCity";
        private const String USER_PASS = "BikeInCity";
        private const String DB_NAME = "BikeInCity";
        #endregion

        private static ISessionFactory SessionFactory;


        static void Main(string[] args)
        {
            GenerateDatabase(DB_NAME, USER_NAME, DB_CREATION_SCRIPT_FILENAME, USER_PASS);
            GenerateSchema();
            Console.WriteLine("Database and Schema generate");
            Console.WriteLine("Press any key to close");
            Console.ReadKey();
        }

        public static String GenerateDatabase(String dbName, String userName, String sqlFilePath, String userPass)
        {
            String returnMessage;

            try
            {
                String dropCreationScript = File.ReadAllText(sqlFilePath, Encoding.Default);
                dropCreationScript = dropCreationScript.Replace("DATABASE_NAME", dbName);
                dropCreationScript = dropCreationScript.Replace("DATABASE_USER", userName);
                dropCreationScript = dropCreationScript.Replace("DATABASE_PASSWORD", userPass);

                String connectionString = ConfigurationManager.ConnectionStrings["Admin"].ConnectionString;

                using (var connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(dropCreationScript, connection);
                    command.ExecuteNonQuery();
                }

                DynamicConnectionProvider.CurrentDatabaseName = dbName;

                returnMessage = String.Format("Database '{0}' has been dropped and created succesfully.", dbName);
            }
            catch (Exception ex)
            {
                returnMessage = String.Format("An error occured while creating/dropping the database '{0}': {1} \n {2}", dbName, ex.Message, ex);
            }

            return returnMessage;
        }

        public static void GenerateSchema()
        {
            SessionFactory = new SessionFactoryFactory(true).GetSessionFactory();
            CurrentSessionContext.Bind(SessionFactory.OpenSession());
        }
    }
}
