using bleak.Sql.VersionManager.SqlServer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

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
            var database = (SqlServerDatabase)manager.GetDatabase();
            Assert.IsTrue(database.CreateDate > DateTime.Now.AddMinutes(-5));
            Assert.IsTrue(DateTime.Now.AddMinutes(5) > database.CreateDate);
        }

        [TestMethod]
        public void TestDatabaseHasTables()
        {
            var database = (SqlServerDatabase)manager.GetDatabase();

            Assert.IsTrue(database.Tables.Count >= 1);
        }

        [TestMethod]
        public void TestVersionTableCreated()
        {
            var database = (SqlServerDatabase)manager.GetDatabase();
            var tables = database.Tables.Cast<SqlServerTable>();
            Assert.AreEqual(tables.Count(), 1);
            Assert.IsTrue(tables.Any(x => x.Name == "Log" && x.Schema == "version"));
        }

        [TestMethod]
        public void TestVersionTableStructure()
        {
            var database = (SqlServerDatabase)manager.GetDatabase();
            var table = database.Tables
                .Cast<SqlServerTable>()
                .FirstOrDefault(x => x.Name == "Log" && x.Schema == "version")
                ;
            Assert.IsTrue(table.Columns.Any(c => c.Name == "Script"));
            Assert.IsTrue(table.Columns.Any(c => c.Name == "DeployDate"));
        }
    }
}