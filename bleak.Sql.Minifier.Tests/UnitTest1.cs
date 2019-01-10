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
            Assert.IsFalse(results.Contains(";"));
        }

        [TestMethod]
        public void Formatter_Simple_Select()
        {
            var sql = "SElect * from Employee";
            var minifier = new SqlMinifier();
            var results = minifier.JamalFormat(sql);
            Assert.IsFalse(results.Contains(";"));
        }

        [TestMethod]
        public void Formatter_Ultimate_Test()
        {
            var sql = "SELECT zid , shopper_id , session_start_date , session_end_date , session_length_minutes , session_length_seconds , is_purchase FROM ( SELECT DISTINCT zid , shopper_id , CAST(NULL AS Datetime) as session_start_date , session_end_date , CAST(NULL AS BIGINT) AS session_length_minutes , CAST(NULL AS BIGINT) AS session_length_seconds , 1 AS is_purchase FROM ( SELECT purchase_events.sid , purchase_events.zid , DATEADD(ms, purchase_events.createdat - datediff(ms, '1970-01-01', getdate()), GETDATE()) as session_end_date , si3.shopper_id FROM ( SELECT sid, zid, createdat FROM data_onboarding.web_purchase_confirm WHERE zid IS NOT NULL ) purchase_events JOIN ( SELECT shopper_id , id_value as zid FROM shopper360.shopper_identifier WHERE id_type = 'zid' ) si3 ON si3.zid = purchase_events.zid ) synthesized_purchase_events ) ins_table";
            var minifier = new SqlMinifier();
            var results = minifier.JamalFormat(sql);
            Assert.IsFalse(results.Contains(";"));
        }
    }
}
