using bleak.Sql.Minifier;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace bleak.Sql.VersionManager
{
    public class Class1 { }

    public class bleakSqlContext : DbContext
    {
        public MasterDbContext() : base() { }
        public bleakSqlContext(DbContextOptions<bleakSqlContext> options) : base(options)
        { }
        public virtual DbSet<VersionLog> VersionLogs { get; set; }

        
    }
    public class VersionLog
    {
        public string ScriptName { get; set; }
        public DateTime DeployDate { get; set; }
    }
    public class SqlServerVersionManager
    {
        public string Folder { get; private set; }
        public string Server { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public SqlServerVersionManager(string folder, string server, string username, string password)
        {
            Folder = folder;
            Password = password;
            Username = username;
            Server = server;
            DirSearch(Folder);
        }
        public IList<Script> Scripts { get; set; } = new List<Script>();
        public void Execute()
        {

        }

        public void CreateDatabase(string databaseName)
        {
            database
            }

        private void DirSearch(string sDir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d).OrderBy(s => s))
                    {
                        if (f.EndsWith(".sql"))
                        {
                            var script = new Script();
                            script.FileName = f;
                            Scripts.Add(script);
                        }
                        Console.WriteLine(f);
                    }
                    DirSearch(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }
    }

    public class Minifier
    {
        public SqlMinifier SqlMinifier { get; set; }

        private Minifier()
        {
            SqlMinifier = new SqlMinifier();
        }
        private static object syncroot = new object();
        private static Minifier _instance;
        public static Minifier Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncroot)
                    {
                        if (_instance == null)
                        {
                            _instance = new Minifier();
                        }
                    }
                }
                return _instance;
            }
        }
    }

    public static class StringExtensionMethods
    {
        public static string Minify(this string input)
        {
            return Minifier.Instance.SqlMinifier.Minify(input);
        }
    }
    public class Script
    {
        public int Index { get; set; }
        public string FileName { get; set; }
        public string LoadFullText(bool minify = true)
        {
            string data = File.ReadAllText(FileName);
            if (minify)
            {
                return data.Minify();
            }
            return data;
        }
    }
}
