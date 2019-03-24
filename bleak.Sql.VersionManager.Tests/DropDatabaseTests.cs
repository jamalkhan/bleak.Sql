using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bleak.Sql.VersionManager.Tests
{

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
}