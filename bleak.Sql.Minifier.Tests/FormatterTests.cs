﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

namespace bleak.Sql.Minifier.Tests
{
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
            var sql = File.ReadAllText(@"Formatter_Nested_Select_Test_Input.sql");
            var minifier = new SqlMinifier(tab: "    ");
            var results = minifier.JamalFormat(sql);
            var expected = File.ReadAllText(@"Formatter_Nested_Select_Test_Expected.sql");
            Assert.AreEqual(expected, results);
        }

        [TestMethod]
        public void Formatter_Ultimate_Test()
        {
            var sql = File.ReadAllText(@"Formatter_Ultimate_Test_Input.sql");
            var minifier = new SqlMinifier(tab: "    ");
            var results = minifier.JamalFormat(sql);
            string expected = File.ReadAllText(@"Formatter_Ultimate_Test_Expected.sql");
            Assert.AreEqual(expected, results);
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