using bleak.Sql.VersionManager.Redshift.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace bleak.Sql.VersionManager.Redshift.Tests
{
    public class UnitTest1 { }

    public abstract class TestBaseClass
    {
        protected RedshiftVersionManager versionManager;
        protected string Folder = Path.Combine(Directory.GetCurrentDirectory(), "Scripts");

        [TestCleanup]
        public virtual void Dispose()
        {
        }
        [TestInitialize]
        public virtual void Initialize()
        {

        }


        protected TestBaseClass()
        {
            versionManager = new RedshiftVersionManager(
                scriptRepo: new FileSystemScriptRepo(Folder),
                host: Configuration.Settings.Master.Host,
                port: Configuration.Settings.Master.Port,
                database: Configuration.Settings.Master.Database,
                username: Configuration.Settings.Master.Username,
                password: Configuration.Settings.Master.Password);
        }
    }

    [TestClass]
    public class RedshiftDatabaseScannerTests:TestBaseClass
    {
        public RedshiftDatabaseScannerTests() : base()
        {

        }
        [TestMethod]
        public void TestMethod1()
        {
            var database = (RedshiftDatabase)versionManager.GetDatabase();
            Assert.IsTrue(database != null);
            Assert.IsTrue(database.Tables.Count > 0);
            Assert.IsTrue(database.Schemas.Count > 0);
            Assert.IsTrue(database.Tables[0].Columns.Count > 0);
        }

        [TestMethod]
        public void InitializeDatabaseTest()
        {
            versionManager.IntializeDatabase();
            var database = (RedshiftDatabase)versionManager.GetDatabase();
            Assert.IsTrue(database.Schemas
                .Count(s => s.Name == "version") == 1);
            Assert.IsTrue(database.Tables
                .Cast<RedshiftTable>()
                .Count(s => s.Schema== "version" && s.Name == "log") == 1);
        }
        [TestMethod]
        public void MyTestMethod()
        {
            versionManager.UpdateDatabase();
        }
    }
}
