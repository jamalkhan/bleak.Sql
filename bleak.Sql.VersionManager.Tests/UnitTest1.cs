using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace bleak.Sql.VersionManager.Tests
{
    [TestClass]
    public class UnitTest1
    {
    }
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
            var database = manager.GetDatabase();
            Assert.IsTrue(database.Tables.Contains("Employee", "dbo"));
            var versionLog = manager.GetDeployedChangesets();
            Assert.IsTrue(versionLog.Any(x => x.Script.Contains("001_Create_Employee.sql")));
            Assert.IsTrue(versionLog.Any(x => x.Script.Contains("002_Alter_Employee.sql")));
            Assert.AreEqual(versionLog.Count, 2);
            
            try
            {
                var sql = "CREATE TABLE dbo.Employer ( Id BIGINT IDENTITY(1, 1), Name VARCHAR(500), Address VARCHAR(500));";
                File.WriteAllLines(Path.Combine(Folder, "003_Create_Employeer.sql"), new string[] { sql });
                manager.UpdateDatabase();
                versionLog = manager.GetDeployedChangesets();
                Assert.IsTrue(versionLog.Any(x => x.Script.Contains("003_Create_Employeer.sql")));
                Assert.AreEqual(versionLog.Count, 3);
            }
            finally
            {
                File.Delete(Path.Combine(Folder, "003_Create_Employeer.sql"));
            }
        }

        public override void Dispose()
        {
            File.Delete(Path.Combine(Folder, "003_Create_Employeer.sql"));
            base.Dispose();
        }
    }
}
