using bleak.Sql.VersionManager.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace bleak.Sql.VersionManager.Tests
{
    [TestClass]
    public class VersionManagementTests : TestBaseClass
    {
        public VersionManagementTests() : base("_version_management_test_")
        {
        }

        [TestMethod]
        public void UpdateToLatestVersion()
        {
            manager.UpdateDatabase();
            var database = (SqlServerDatabase)manager.GetDatabase();
            var tables = database.Tables.Cast<SqlServerTable>();
            Assert.IsTrue(tables
                .Any(t => t.Name == "Employee" && t.Schema == "dbo"));
            var versionLog = manager.GetDeployedChangesets();
            Assert.IsTrue(versionLog.Any(x => x.Script.Contains("001_Create_Employee.sql")));
            Assert.IsTrue(versionLog.Any(x => x.Script.Contains("002_Alter_Employee.sql")));
            Assert.AreEqual(versionLog.Count, 2);

            var sql = "CREATE TABLE dbo.Employer ( Id BIGINT IDENTITY(1, 1), Name VARCHAR(500), Address VARCHAR(500));";
            File.WriteAllLines(Path.Combine(Folder, "003_Create_Employeer.sql"), new string[] { sql });
            manager.UpdateDatabase();
            versionLog = manager.GetDeployedChangesets();
            Assert.IsTrue(versionLog.Any(x => x.Script.Contains("003_Create_Employeer.sql")));
            Assert.AreEqual(versionLog.Count, 3);
        }

        [TestCleanup]
        public override void Dispose()
        {
            File.Delete(Path.Combine(Folder, "003_Create_Employeer.sql"));
            manager.DropDatabase();
        }
    }
}