using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bleak.Sql.Minifier.Tests
{
    [TestClass]
    public class MinifierTests
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
        public void Remove_Double_Space_From_Complicated_Select()
        {
            var sql = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\nSELECT\r\n\tId\r\n,\tFirstName\r\n,\tLastName\r\n,\tHireDate\r\n,\tFireDate\r\nFROM Employee";
            var minifier = new SqlMinifier();
            var results = minifier.Minify(sql);
            Assert.IsFalse(results.Contains("  "));
        }

        [TestMethod]
        public void Complicated_Select_Does_Not_End_In_Space()
        {
            var sql = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\nSELECT\r\n\tId\r\n,\tFirstName\r\n,\tLastName\r\n,\tHireDate\r\n,\tFireDate\r\nFROM Employee";
            var minifier = new SqlMinifier();
            var results = minifier.Minify(sql);
            Assert.IsFalse(results.EndsWith(' '));
        }

        [TestMethod]
        public void Complicated_Select_Does_Not_Contain_Space_Then_Comma()
        {
            var sql = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\nSELECT\r\n\tId\r\n,\tFirstName\r\n,\tLastName\r\n,\tHireDate\r\n,\tFireDate\r\nFROM Employee";
            var minifier = new SqlMinifier();
            var results = minifier.Minify(sql);
            Assert.IsFalse(results.Contains(" ,"));
        }

        [TestMethod]
        public void Remove_Tab_From_Simple_Select()
        {
            var sql = "SELECT *\r\n\tFROM Employee";
            var minifier = new SqlMinifier();
            var results = minifier.Minify(sql);
            Assert.IsFalse(results.Contains("\t"));
        }

        [TestMethod]
        public void Uppercase_Reserved_Word_Select()
        {
            var sql = "select *\r\n\tfrom Employee";
            var minifier = new SqlMinifier();
            var results = minifier.Minify(sql);
            Assert.IsFalse(results.Contains("SELECT"));
        }

        [TestMethod]
        public void Uppercase_Reserved_Word_From()
        {
            var sql = "select *\r\n\tfrom Employee";
            var minifier = new SqlMinifier();
            var results = minifier.Minify(sql);
            Assert.IsFalse(results.Contains("FROM"));
        }

        [TestMethod]
        public void Two_Statements_Divided_By_Semicolon()
        {
            var sql = "select *\r\n\tfrom Employee; select *\r\n\tfrom Employee;";
            var minifier = new SqlMinifier();
            var results = minifier.Minify(sql);
            Assert.IsTrue(results.Contains(";"));
        }
    }
}