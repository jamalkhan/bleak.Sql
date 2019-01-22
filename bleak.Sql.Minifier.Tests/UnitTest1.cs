using Microsoft.VisualStudio.TestTools.UnitTesting;
using bleak.Sql.Minifier;
using System.IO;
using System.Collections.Generic;

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
    [TestClass]
    public class FormatterTests
    {
        [TestMethod]
        public void Formatter_Select_Star_From_Employee_Test()
        {
            var sql = "SElect * from Employee";
            var minifier = new SqlMinifier();
            var results = minifier.JamalFormat(sql);
            Assert.IsTrue(results.Equals("SELECT\r\n\t*\r\nFROM Employee"));
        }

        [TestMethod]
        public void Formatter_Select_Star_From_Employee_Semicolon_Test()
        {
            var sql = "SElect * from Employee;";
            var minifier = new SqlMinifier();
            var results = minifier.JamalFormat(sql);
            Assert.IsTrue(results.Equals("SELECT\r\n\t*\r\nFROM Employee;"));
        }

        [TestMethod]
        public void Formatter_Select_Distinct_Star_From_Employee_Semicolon_Test()
        {
            var sql = "SElect DISTINCT * from Employee;";
            var minifier = new SqlMinifier();
            var results = minifier.JamalFormat(sql);
            Assert.IsTrue(results.Equals("SELECT DISTINCT\r\n\t*\r\nFROM Employee;"));
        }

        [TestMethod]
        public void Formatter_Select_Top_50_Star_From_Employee_Semicolon_Test()
        {
            var sql = "SElect TOP 50 * from Employee;";
            var minifier = new SqlMinifier();
            var results = minifier.JamalFormat(sql);
            Assert.IsTrue(results.Equals("SELECT TOP 50\r\n\t*\r\nFROM Employee;"));
        }

        [TestMethod]
        public void Formatter_Nested_Select_Test()
        {
            var sql = "SElect TOP 50 * from ( SELECT 'Jamal  H' AS [First Name], 'Khan' AS [Last Name] FROM Employee ) x;";
            var minifier = new SqlMinifier(tab: "\t");
            var results = minifier.JamalFormat(sql);
            var expected = "SELECT TOP 50\r\n\t*\r\nFROM\r\n(\r\n\tSELECT\r\n\t\t'Jamal  H' AS [First Name]\r\n\t,\t'Khan' AS [Last Name]\r\n\tFROM\r\n\tEmployee\r\n) x;";
            Assert.AreEqual(expected, results);
        }

        [TestMethod]
        public void Formatter_Ultimate_Test()
        {
            var sql = "SELECT zid , shopper_id , session_start_date , session_end_date , session_length_minutes , session_length_seconds , is_purchase FROM ( SELECT DISTINCT zid , shopper_id , CAST(NULL AS Datetime) as session_start_date , session_end_date , CAST(NULL AS BIGINT) AS session_length_minutes , CAST(NULL AS BIGINT) AS session_length_seconds , 1 AS is_purchase FROM ( SELECT purchase_events.sid , purchase_events.zid , DATEADD(ms, purchase_events.createdat - datediff(ms, '1970-01-01', getdate()), GETDATE()) as session_end_date , si3.shopper_id FROM ( SELECT sid, zid, createdat FROM data_onboarding.web_purchase_confirm WHERE zid IS NOT NULL ) purchase_events JOIN ( SELECT shopper_id , id_value as zid FROM shopper360.shopper_identifier WHERE id_type = 'zid' ) si3 ON si3.zid = purchase_events.zid ) synthesized_purchase_events ) ins_table";
            var minifier = new SqlMinifier(tab: "    ");
            var results = minifier.JamalFormat(sql);
            string contents = File.ReadAllText(@"Ultimate_Format_Test.sql");
            Assert.AreEqual(contents, results);
        }

        [TestMethod]
        public void Handle_Cast_DateTime()
        {
            string[] cast = new string[] { "CAST", "(", "NULL", "AS", "DATETIME", ")" };
            var minifier = new SqlMinifier();
            var results = minifier.HandleCast(cast);
            Assert.AreEqual(results, "CAST(NULL AS DATETIME)");
        }

        [TestMethod]
        public void Handle_Cast_String()
        {
            string[] cast = new string[] { "CAST", "(", "NULL", "AS", "VARCHAR", "(", "50", ")", ")" };
            var minifier = new SqlMinifier();
            var results = minifier.HandleCast(cast);
            Assert.AreEqual(results, "CAST(NULL AS VARCHAR(50))");
        }

        [TestMethod]
        public void Extract_Cast_String_Test()
        {
            var sql = "SELECT CAST('Jamal' AS VARCHAR(50)) AS FirstName FROM [dbo].[Employee];";
            var minifier = new SqlMinifier();
            var sqlWords = minifier.LoadWordArray(sql);
            var castPosition = 1;
            Assert.IsTrue(sqlWords[castPosition] == "CAST");
            var cast1 = minifier.GetCast(sqlWords, ref castPosition, sql_function: "CAST");
            Assert.AreEqual(cast1.Length, 9);
            Assert.AreEqual(castPosition, 9);
            var results = minifier.HandleCast(cast1);
            Assert.AreEqual(results, "CAST('Jamal' AS VARCHAR(50))");
        }


        [TestMethod]
        public void Extract_Where_String_Test()
        {
            var sql = "SELECT CAST('Jamal' AS VARCHAR(50)) AS FirstName FROM [dbo].[Employee] WHERE LastName IS NOT NULL;";
            var minifier = new SqlMinifier();
            var sqlWords = minifier.LoadWordArray(sql);
            var castPosition = 16;
            Assert.IsTrue(sqlWords[castPosition] == "WHERE");
            var cast1 = minifier.GetWhere(sqlWords, ref castPosition, sql_function: "WHERE");
            Assert.AreEqual(cast1.Length, 5);
            Assert.AreEqual(castPosition, 20);
            var results = minifier.HandleCast(cast1);
            Assert.AreEqual(results, "WHERE LastName IS NOT NULL");
        }

        [TestMethod]
        public void Get_Parentheses_Test()
        {
            var sql = "SELECT * FROM [dbo].[Employee] WHERE LastName IN ('Khan', 'Smith');";
            var minifier = new SqlMinifier();
            var sqlWords = minifier.LoadWordArray(sql);
            var castPosition = 9;
            Assert.IsTrue(sqlWords[castPosition] == "(");
            var output = new List<string>();
            minifier.GetParantheses(sqlWords, startingPosition: ref castPosition, output: ref output);
            Assert.AreEqual(output.Count, 5);
            Assert.AreEqual(castPosition, 13);
            var results = minifier.HandleCast(output.ToArray());
            Assert.AreEqual(results, "('Khan', 'Smith')");
        }

        [TestMethod]
        public void Get_Between_Test()
        {
            var sql = "SELECT * FROM [dbo].[Employee] WHERE StartDate BETWEEN '1/21/2018' AND '1/21/2019'";
            var minifier = new SqlMinifier();
            var sqlWords = minifier.LoadWordArray(sql);
            var castPosition = 8;
            Assert.IsTrue(sqlWords[castPosition] == "BETWEEN");
            var output = new List<string>();
            minifier.GetBetween(sqlWords, startingPosition: ref castPosition, output: ref output);
            Assert.AreEqual(output.Count, 4);
            Assert.AreEqual(castPosition, 12);
            var results = minifier.HandleCast(output.ToArray());
            Assert.AreEqual(results, "BETWEEN '1/21/2018' AND '1/21/2019'");
        }

        [TestMethod]
        public void Load_Word_Array_Test_1()
        {
            string sql = "SELECT * FROM Employee;";
            var minifier = new SqlMinifier();
            var results = minifier.LoadWordArray(sql);
            Assert.AreEqual(results[0], "SELECT");
            Assert.AreEqual(results[1], "*");
            Assert.AreEqual(results[2], "FROM");
            Assert.AreEqual(results[3], "Employee");
            Assert.AreEqual(results[4], ";");
        }

        [TestMethod]
        public void Load_Word_Array_Test_2()
        {
            string sql = "SELECT 'Jamal H' AS [First Name],  'Khan' AS [Last  Name] FROM [dbo].[Master Employee];";
            var minifier = new SqlMinifier();
            var results = minifier.LoadWordArray(sql);
            Assert.AreEqual(results[0], "SELECT");
            Assert.AreEqual(results[1], "'Jamal H'");
            Assert.AreEqual(results[2], "AS");
            Assert.AreEqual(results[3], "[First Name]");
            Assert.AreEqual(results[4], ",");
            Assert.AreEqual(results[5], "'Khan'");
            Assert.AreEqual(results[6], "AS");
            Assert.AreEqual(results[7], "[Last  Name]");
            Assert.AreEqual(results[8], "FROM");
            Assert.AreEqual(results[9], "[dbo]");
            Assert.AreEqual(results[10], ".");
            Assert.AreEqual(results[11], "[Master Employee]");
            Assert.AreEqual(results[12], ";");
        }

        [TestMethod]
        public void Load_Word_Array_Test_3()
        {
            string sql = "SELECT * FROM(SELECT 'Jamal H' AS [First Name],  'Khan' AS [Last  Name] FROM [dbo].[Master Employee])innerTable;";
            var minifier = new SqlMinifier();
            var results = minifier.LoadWordArray(sql);
            Assert.AreEqual(results[0], "SELECT");
            Assert.AreEqual(results[1], "*");
            Assert.AreEqual(results[2], "FROM");
            Assert.AreEqual(results[3], "(");
            Assert.AreEqual(results[4], "SELECT");
            Assert.AreEqual(results[5], "'Jamal H'");
            Assert.AreEqual(results[6], "AS");
            Assert.AreEqual(results[7], "[First Name]");
            Assert.AreEqual(results[8], ",");
            Assert.AreEqual(results[9], "'Khan'");
            Assert.AreEqual(results[10], "AS");
            Assert.AreEqual(results[11], "[Last  Name]");
            Assert.AreEqual(results[12], "FROM");
            Assert.AreEqual(results[13], "[dbo]");
            Assert.AreEqual(results[14], ".");
            Assert.AreEqual(results[15], "[Master Employee]");
            Assert.AreEqual(results[16], ")");
            Assert.AreEqual(results[17], "innerTable");
            Assert.AreEqual(results[18], ";");
        }
    }
}