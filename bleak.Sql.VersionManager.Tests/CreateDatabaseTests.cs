using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace bleak.Sql.VersionManager.Tests
{

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
}