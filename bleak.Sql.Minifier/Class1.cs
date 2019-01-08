﻿using System;
using System.Linq;
using System.Text;

namespace bleak.Sql.Minifier
{
    public class SqlMinifier
    {
        string[] ReservedWords = {
            "ADD", "ADD CONSTRAINT", "ALTER", "ALTER COLUMN", "ALTER TABLE", "ALL", "AND", "ANY", "AS", "ASC", "BACKUP DATABASE", "BETWEEN", "CASE", "CHECK", "COLUMN", "CONTRAINT",
            "CREATE", "CREATE DATABASE", "CREATE INDEX", "CREATE OR REPLACE VIEW", "CREATE TABLE", "CREATE PROCEDURE", "CREATE UNIQUE INDEX", "CREATE VIEW",
                      "DATABASE", "DEFAULT", "DELETE", "DESC", "DESTINCT", "DROP", "DROP COLUMN", "DROP CONSTRAINT", "DROP DATABASE", "DROP DEFAULT", "DROP INDEX", "DROP TABLE",
                      "DROP VIEW", "EXEC", "EXISTS", "FOREIGN KEY", "FROM", "FULL OUTER JOIN", "GROUP BY", "HAVING", "IN", "INDEX", "INNER JOIN", "INSERT INTO", "INSERT INTO SELECT",
                      "IS NULL", "IS NOT NULL", "JOIN", "LEFT JOIN", "LIKE", "LIMIT", "NOT", "NOT NULL", "OR", "ORDER BY", "OUTER JOIN", "PRIMARY KEY", "PROCEDURE", "RIGHT JOIN",
                      "ROWNUM", "SELECT", "SELECT DISTINCT", "SELECT INTO", "SELECT TOP", "SET", "TABLE", "TOP", "TRUNCATE TABLE", "UNION", "UNION ALL", "UNIQUE", "UPDATE", "VALUES", "VIEW", "WHERE"
            };

        public string Minify(string sql)
        {
            var cleanedSql = sql.Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ")
                .Replace("  ", " ");

            var sb = new StringBuilder();

            var cleanedSqlWordArray = cleanedSql.Split(' ');
            foreach (var word in cleanedSqlWordArray)
            {
                if (word.Trim().Length > 0)
                {
                    if (word == ",")
                    {
                        sb.Remove(sb.Length - 1, 1);
                    }
                    if (ReservedWords.Contains(word))
                    {
                        sb.Append(word.ToUpper());
                    }
                    else
                    {
                        sb.Append(word);
                    }
                    sb.Append(" ");
                }
            }
            return sb.ToString()
                .Trim();
        }
    }
}
