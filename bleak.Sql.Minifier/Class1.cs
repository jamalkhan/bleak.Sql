﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace bleak.Sql.Minifier
{
    public class Wordset
    {
        public int Index { get; set; }
        public string CurrentWord { get; set; }
        public string UppercaseCurrentWord { get; set; }

        public string PreviousWord { get; set; }
        public string UppercasePreviousWord { get; set; }

        public string NextWord { get; set; }
        public string UppercaseNextWord { get; set; }
    }

    public class SqlMinifier
    {
        private readonly string _lineEnd;
        public SqlMinifier(string lineEnd = "\r\n")
        {
            _lineEnd = lineEnd;
        }

        private string[] ReservedWords = {
            "ADD", "ADD CONSTRAINT", "ALTER", "ALTER COLUMN", "ALTER TABLE", "ALL", "AND", "ANY", "AS", "ASC", "BACKUP DATABASE", "BETWEEN", "CASE", "CHECK", "COLUMN", "CONTRAINT",
            "CREATE", "CREATE DATABASE", "CREATE INDEX", "CREATE OR REPLACE VIEW", "CREATE TABLE", "CREATE PROCEDURE", "CREATE UNIQUE INDEX", "CREATE VIEW",
                      "DATABASE", "DEFAULT", "DELETE", "DESC", "DESTINCT", "DROP", "DROP COLUMN", "DROP CONSTRAINT", "DROP DATABASE", "DROP DEFAULT", "DROP INDEX", "DROP TABLE",
                      "DROP VIEW", "EXEC", "EXISTS", "FOREIGN KEY", "FROM", "FULL OUTER JOIN", "GROUP BY", "HAVING", "IN", "INDEX", "INNER JOIN", "INSERT INTO", "INSERT INTO SELECT",
                      "IS NULL", "IS NOT NULL", "JOIN", "LEFT JOIN", "LIKE", "LIMIT", "NOT", "NOT NULL", "OR", "ORDER BY", "OUTER JOIN", "PRIMARY KEY", "PROCEDURE", "RIGHT JOIN",
                      "ROWNUM", "SELECT", "SELECT DISTINCT", "SELECT INTO", "SELECT TOP", "SET", "TABLE", "TOP", "TRUNCATE TABLE", "UNION", "UNION ALL", "UNIQUE", "UPDATE", "VALUES", "VIEW", "WHERE"
            };

        public string[] LoadWordArray(string sql)
        {
            var words = new List<string>();
            var regex = new Regex("([\\s.,;()])");
            var split = regex.Split(sql).Where(s => !string.IsNullOrEmpty(s)).ToArray();
            var delimitedSplit = new List<string>();
            for (int i = 0; i < split.Count(); i++)
            {
                var word = split[i];
                if (word.StartsWith("'"))
                {
                    var ticksb = new StringBuilder();
                    while (!word.EndsWith("'"))
                    {
                        ticksb.Append(word);
                        word = split[++i];
                    }
                    ticksb.Append(word);
                    delimitedSplit.Add(ticksb.ToString());
                }
                else if (word.StartsWith("\""))
                {
                    var ticksb = new StringBuilder();
                    while (!word.EndsWith("\""))
                    {
                        ticksb.Append(word);
                        word = split[++i];
                    }
                    ticksb.Append(word);
                    delimitedSplit.Add(ticksb.ToString());
                }
                else if (word.StartsWith("["))
                {
                    var ticksb = new StringBuilder();
                    while (!word.EndsWith("]"))
                    {
                        ticksb.Append(word);
                        word = split[++i];
                    }
                    ticksb.Append(word);
                    delimitedSplit.Add(ticksb.ToString());
                }
                else
                {
                    delimitedSplit.Add(word);
                }
            }

            return delimitedSplit.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        }

        private static List<string> SemicolonSplit(List<string> spaceSplit, string splitter)
        {
            var words = new List<string>();
            for (int i = 0; i < spaceSplit.Count; i++)
            {
                var word = spaceSplit[i];
                if (word.Contains(splitter))
                {
                    var subsplit = word.Split(splitter.ToCharArray());
                    foreach (var subword in subsplit)
                    {
                        words.Add(string.IsNullOrWhiteSpace(subword) ? splitter : subword);
                    }
                }
                else
                {
                    words.Add(word);
                }
            }

            return words;
        }

        private static void HandleQuotes(List<string> words, List<string> spaceSplit, ref int i, ref string word, string delimiter, string endlimiter = null)
        {
            if (endlimiter == null)
            {
                endlimiter = delimiter;
            }

            if (word.StartsWith(delimiter) && !word.EndsWith(endlimiter))
            {
                var sb = new StringBuilder();
                sb.Append(word);
                do
                {
                    word = spaceSplit[++i];
                    if (word.Contains(endlimiter) && !word.EndsWith(endlimiter))
                    {
                        sb.Append(word);
                    }
                    else
                    {
                        sb.Append(word);
                    }
                } while (!word.Contains(endlimiter));
                words.Add(sb.ToString());
                word = spaceSplit[++i];
            }
            else if (word.StartsWith(delimiter) && !word.EndsWith(endlimiter))
            {
                words.Add(word);
            }
        }

        public string Minify(string sql)
        {
            var cleanedSql = sql.Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ")
                .Replace("  ", " ");

            var sb = new StringBuilder();

            var cleanedSqlWordArray = LoadWordArray(cleanedSql);
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

        public string[] GetCast(string[] sqlWords, ref int startingPosition)
        {
            if (sqlWords[startingPosition] != "CAST")
            {
                throw new ArgumentOutOfRangeException();
            }
            var retval = new List<string>();
            retval.Add(sqlWords[startingPosition++]);
            var terminatedDepth = 0;
            do
            {
                var word = sqlWords[startingPosition];
                retval.Add(sqlWords[startingPosition]);
                if (word == "(")
                {
                    terminatedDepth++;
                }
                if (word == ")")
                {
                    terminatedDepth--;
                }
                startingPosition++;
            } while (terminatedDepth > 0);
            return retval.ToArray();
        }

        public string JamalFormat(string sql)
        {
            var sqlWords = LoadWordArray(sql);

            var sb = new StringBuilder();

            int baseTab = 0;
            for (int i = 0; i < sqlWords.Length; i++)
            {
                var word = sqlWords[i];

                if (ReservedWords.Contains(word.ToUpper()))
                {
                    word = word.ToUpper();
                }

                switch (word.ToUpper())
                {
                    case "SELECT":
                        var nextWord = sqlWords[i + 1];
                        switch (nextWord.ToUpper())
                        {
                            case "TOP":
                                sb.Append(word);
                                sb.Append(" ");
                                i++;
                                sb.Append(sqlWords[i].ToUpper());
                                sb.Append(" ");
                                i++;
                                sb.Append(sqlWords[i]);
                                sb.Append(_lineEnd);
                                break;
                            case "DISTINCT":
                                sb.Append(word);
                                sb.Append(" ");
                                i++;
                                sb.Append(sqlWords[i].ToUpper());
                                sb.Append(_lineEnd);
                                break;
                            default:
                                sb.Append(word);
                                sb.Append(_lineEnd);
                                break;
                        }
                        AddBaseTab(sb, baseTab);
                        sb.Append("\t");
                        break;
                    case "CAST":
                        var castWords = GetCast(sqlWords, ref i);
                        var cast = HandleCast(castWords);
                        sb.Append(cast);
                        sb.Append(" ");
                        break;
                    case ";":
                        RemoveTrailingSpaces(sb);
                        sb.Append(word);
                        break;
                    case "FROM":
                        RemoveTrailingSpaces(sb);
                        sb.Append(_lineEnd);
                        AddBaseTab(sb, baseTab);
                        sb.Append(word);
                        sb.Append(" ");
                        break;
                    case "(":


                        break;
                    default:
                        sb.Append(word);
                        sb.Append(" ");
                        break;
                }




            }
            return sb.ToString()
                .Trim();
        }

        private static void RemoveTrailingSpaces(StringBuilder sb)
        {
            while (sb.ToString().EndsWith(" "))
            {
                sb.Remove(sb.Length - 1, 1);
            }
        }

        public string HandleCast(string[] castWords)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < castWords.Length; i++)
            {
                var word = castWords[i];
                sb.Append(word);
                if (castWords.Length > i + 1)
                {
                    switch (castWords[i + 1])
                    {
                        case "(":
                        case ")":
                            break;
                        default:
                            if (word == "(")
                            {

                            }
                            else
                            {
                                sb.Append(" ");
                            }
                            break;
                    }
                }
            }
            return sb.ToString();
        }


        private static Wordset LoadWords(string[] cleanedSqlWordArray, int i)
        {
            string previous_word = null;
            string uprevious_word = null;
            if (i - 1 > 0)
            {
                previous_word = cleanedSqlWordArray[i - 1];
                uprevious_word = previous_word.ToUpperInvariant();
            }

            string word = cleanedSqlWordArray[i];
            string uword = word.ToUpperInvariant();
            string next_word = null;
            string unext_word = null;
            if (i + 1 < cleanedSqlWordArray.Length)
            {
                next_word = cleanedSqlWordArray[i + 1];
                unext_word = next_word.ToUpperInvariant();
            }

            return new Wordset()
            {
                Index = i,
                NextWord = next_word,
                UppercaseNextWord = unext_word,
                CurrentWord = word,
                UppercaseCurrentWord = uword,
                PreviousWord = previous_word,
                UppercasePreviousWord = uprevious_word,
            };
        }

        private void AddBaseTab(StringBuilder sb, int baseTab)
        {
            for (int i = 0; i < baseTab; i++)
            {
                sb.Append("\t");
            }
        }
    }
}
