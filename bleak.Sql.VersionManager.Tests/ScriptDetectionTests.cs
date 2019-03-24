using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace bleak.Sql.VersionManager.Tests
{

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