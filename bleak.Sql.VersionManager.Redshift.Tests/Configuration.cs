using Microsoft.Extensions.Configuration;
using System.IO;

namespace bleak.Sql.VersionManager.Redshift.Tests
{
    public class Configuration
    {
        private static object syncroot = new object();
        private static AppSettings _instance = null;
        public static AppSettings Settings
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncroot)
                    {
                        if (_instance == null)
                        {
                            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json",
                                    optional: false,
                                    reloadOnChange: true)
                                .AddEnvironmentVariables();

                            IConfigurationRoot configuration = builder.Build();

                            var settings = new AppSettings();
                            configuration.Bind(settings);
                            _instance = settings;
                        }
                    }
                }
                return _instance;
            }
        }
    }
}