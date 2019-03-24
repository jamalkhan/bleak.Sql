using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        }

    }
}
