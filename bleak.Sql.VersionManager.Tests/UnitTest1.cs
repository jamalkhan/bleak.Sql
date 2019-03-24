using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace bleak.Sql.VersionManager.Tests
{
    [TestClass]
    public class UnitTest1
    {
    }


    public abstract class TestBaseClass
    {
        protected string DatabaseName;
        protected SqlServerVersionManager manager;
        [TestCleanup]
        public void Dispose()
        {
            manager.DropDatabase();
        }


        protected TestBaseClass(string testName)
        {
            string dbName = Configuration.Settings.Master.Database + testName + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            manager = new SqlServerVersionManager(
                            folder: Directory.GetCurrentDirectory(),
                            server: Configuration.Settings.Master.Server,
                            username: Configuration.Settings.Master.Username,
                            password: Configuration.Settings.Master.Password,
                            databaseName: dbName,
                            createDatabase: true
                            );
            DatabaseName = dbName;
        }
    }
    [TestClass]
    public class VersionManagementTests : TestBaseClass
    {
        public VersionManagementTests() : base("_version_management_test_")
        {
        }
    }

    [TestClass]
    public class CreateDatabaseTests : TestBaseClass
    {
        public CreateDatabaseTests() : base("_create_test_")
        {
        }

        [TestMethod]
        public void TestCreateDatabase()
        {
            var database = manager.GetDatabase();
            Assert.IsTrue(database.CreateDate > DateTime.Now.AddMinutes(-5));
            Assert.IsTrue(DateTime.Now.AddMinutes(5) > database.CreateDate);
        }

        [TestMethod]
        public void TestDatabaseHasTables()
        {
            var database = manager.GetDatabase();
            Assert.IsTrue(database.Tables.Count >= 1);
        }

        [TestMethod]
        public void TestVersionTableCreated()
        {
            var database = manager.GetDatabase();
            Assert.AreEqual(database.Tables.Count, 1);
            Assert.IsTrue(database.Tables.Contains("Log", "version"));
        }

        [TestMethod]
        public void TestVersionTableStructure()
        {
            var database = manager.GetDatabase();
            var table = database.Tables["Log", "version"];
            Assert.IsTrue(table.Columns.Contains("Script"));
            Assert.IsTrue(table.Columns.Contains("DeployDate"));
        }
    }

    [TestClass]
    public class DropDatabaseTests : TestBaseClass
    {
        public DropDatabaseTests() : base("drop_database_")
        {
        }


        [TestMethod]
        public void CreateDatabase()
        {
            manager.DropDatabase();
            var database = manager.GetDatabase();
            Assert.IsNull(database);
        }
    }

    [TestClass]
    public class ScriptDetectionTests
    {
        private SqlServerVersionManager manager = new SqlServerVersionManager(
            folder: Directory.GetCurrentDirectory(),
            server: Configuration.Settings.Master.Server,
            username: Configuration.Settings.Master.Username,
            password: Configuration.Settings.Master.Password,
            databaseName: null);
        [TestMethod]
        public void TestScriptDetectionCount()
        {
            Assert.AreEqual(manager.Scripts.Count, 2);
        }

        [TestMethod]
        public void TestScriptDetectionScript1()
        {
            Assert.IsTrue(manager.Scripts[0].FileName.EndsWith("001_Create_Employee.sql"));
        }

        [TestMethod]
        public void TestScriptDetectionScript2()
        {
            Assert.IsTrue(manager.Scripts[1].FileName.EndsWith("002_Alter_Employee.sql"));
        }

        [TestMethod]
        public void TestScriptLoad()
        {
            var script = manager.Scripts[1];
            var text = script.LoadFullText();
            Assert.IsTrue(text.Contains("ALTER TABLE dbo.Employee"));
            Assert.IsTrue(text.Contains("ADD StartDate DATETIME;"));
        }

        [TestMethod]
        public void TestScriptLoadNoMinifier()
        {
            var script = manager.Scripts[1];
            var text = script.LoadFullText(minify: false);
            Assert.IsTrue(text.Contains("ALTER TABLE dbo.Employee"));
            Assert.IsTrue(text.Contains("ADD StartDate DATETIME;"));
        }
    }

}
