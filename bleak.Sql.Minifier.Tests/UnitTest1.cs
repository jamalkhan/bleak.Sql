using Microsoft.VisualStudio.TestTools.UnitTesting;
using bleak.Sql.Minifier;

namespace bleak.Sql.Minifier.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void RemoveCRFromSimpleSelect()
        {
            var sql = "SELECT *\r\nFROM Employee";
            var minifier = new SqlMinifier();
            var results = minifier.Minify(sql);
            Assert.IsFalse(results.Contains("\r"));
        }

        [TestMethod]
        public void RemoveLFFromSimpleSelect()
        {
            var sql = "SELECT *\r\nFROM Employee";
            var minifier = new SqlMinifier();
            var results = minifier.Minify(sql);
            Assert.IsFalse(results.Contains("\n"));
        }

        [TestMethod]
        public void RemoveDoubleSpaceFromSimpleSelect()
        {
            var sql = "SELECT *\r\nFROM Employee";
            var minifier = new SqlMinifier();
            var results = minifier.Minify(sql);
            Assert.IsFalse(results.Contains("  "));
        }

        [TestMethod]
        public void RemoveDoubleSpaceFromComplicatedSelect()
        {
            var sql = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\nSELECT\r\n\tId\r\n,\tFirstName\r\n,\tLastName\r\n,\tHireDate\r\n,\tFireDate\r\nFROM Employee";
            var minifier = new SqlMinifier();
            var results = minifier.Minify(sql);
            Assert.IsFalse(results.Contains("  "));
        }

        [TestMethod]
        public void RemoveTabFromSimpleSelect()
        {
            var sql = "SELECT *\r\n\tFROM Employee";
            var minifier = new SqlMinifier();
            var results = minifier.Minify(sql);
            Assert.IsFalse(results.Contains("  "));
        }
    }
}
