using bleak.Sql.VersionManager.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace bleak.Sql.VersionManager.Tests
{
    public abstract class TestBaseClass
    {
        protected string DatabaseName;
        protected SqlServerVersionManager manager;
        protected string Folder = Path.Combine(Directory.GetCurrentDirectory(), "Scripts");

        [TestCleanup]
        public virtual void Dispose()
        {
            manager.DropDatabase();
        }


        protected TestBaseClass(string testName)
        {
            string dbName = Configuration.Settings.Master.Database + testName + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            manager = new SqlServerVersionManager(
                            folder: Folder,
                            server: Configuration.Settings.Master.Server,
                            username: Configuration.Settings.Master.Username,
                            password: Configuration.Settings.Master.Password,
                            databaseName: dbName,
                            createDatabase: true
                            );
            DatabaseName = dbName;
        }
    }
}