﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace bleak.Sql.Minifier
{
    public class SqlMinifier
    {
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
            var cleanedSqlWordArray = LoadWordArray(sql);
            
            var sb = new StringBuilder();

            int baseTab = 0;
            Wordset wordset;
            for (int i = 0; i < cleanedSqlWordArray.Length; i++)
            {
                wordset = LoadWords(cleanedSqlWordArray, i);

                if (wordset.CurrentWord.Trim().Length > 0)
                {
                    if (ReservedWords.Contains(wordset.UppercaseCurrentWord))
                    {
                        sb.Append(wordset.UppercaseCurrentWord);
                        switch (wordset.UppercaseCurrentWord)
                        {
                            case "SELECT":
                                switch (wordset.UppercaseNextWord)
                                {
                                    case "DISTINCT":
                                        wordset = LoadWords(cleanedSqlWordArray, ++i);
                                        sb.Append(" ");
                                        sb.Append(wordset.UppercaseCurrentWord);
                                        sb.Append("\n");
                                        break;
                                    case "TOP":
                                        wordset = LoadWords(cleanedSqlWordArray, ++i);
                                        sb.Append(" ");
                                        sb.Append(wordset.UppercaseCurrentWord);
                                        sb.Append("\n");

                                        wordset = LoadWords(cleanedSqlWordArray, ++i);
                                        sb.Append(" ");
                                        sb.Append(wordset.UppercaseCurrentWord);
                                        break;
                                }
                                break;
                            case "CAST":
                                //sb.Append(HandleCast(cleanedSqlWordArray, i));
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        /*
                        switch (wordset.UppercaseCurrentWord)
                        {
                            case ",":
                                sb.Append(word);
                                AddBaseTab(sb, baseTab);
                                sb.Append("\t");
                                break;
                            case "CAST":

                                sb.Append(word);
                                i++;
                                LoadWords(cleanedSqlWordArray, i)
                                break;
                        }


                        sb.Append(word);
                        */
                    }
                }
            }
            return sb.ToString()
                .Trim();
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

        private class Wordset
        {
            public int Index { get; set; }
            public string CurrentWord { get; set; }
            public string UppercaseCurrentWord { get; set; }

            public string PreviousWord { get; set; }
            public string UppercasePreviousWord { get; set; }

            public string NextWord { get; set; }
            public string UppercaseNextWord { get; set; }
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
