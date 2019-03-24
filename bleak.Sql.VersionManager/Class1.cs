using bleak.Sql.Minifier;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;

namespace bleak.Sql.VersionManager
{
    public class Class1 { }

    public class bleakSqlContext : DbContext
    {
        public bleakSqlContext() : base() { }
        public bleakSqlContext(DbContextOptions<bleakSqlContext> options) : base(options)
        { }
        public virtual DbSet<VersionLog> VersionLogs { get; set; }
    }

    [Table("Log", Schema = "version")]
    public class VersionLog
    {
        public string ScriptName { get; set; }
        public DateTimeOffset DeployDate { get; set; }
    }
    public class SqlServerVersionManager
    {
        public const string _VersionSchema = "version";
        public const string _VersionTable = "log";

        public string Folder { get; private set; }
        public string ServerInstance { get; private set; }
        public string DatabaseName { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public ServerConnection Connection { get; private set; }
        public Server Server { get; private set; }
        public SqlServerVersionManager(
            string folder,
            string server,
            string username,
            string password,
            string databaseName,
            bool createDatabase = false
            )
        {
            Folder = folder;
            Password = password;
            Username = username;
            ServerInstance = server;
            DatabaseName = databaseName;
            Connection = new ServerConnection(serverInstance: ServerInstance, userName: Username, password: Password);
            Server = new Server(Connection);
            if (
                createDatabase
                && !string.IsNullOrEmpty(databaseName)
                && !Server.Databases.Contains(databaseName))
            {
                CreateDatabase();
            }
            DirSearch(Folder);
        }

        public IList<Script> Scripts { get; set; } = new List<Script>();
        public void ExecuteSql(string databaseName, string sql)
        {
            var database = new Database(Server, databaseName);
            database.ExecuteNonQuery(sql);
        }

        public Database CreateDatabase()
        {
            var database = new Database(Server, DatabaseName);
            database.Create();
            IntializeDatabase();
            return GetDatabase();
        }

        public void IntializeDatabase()
        {
            var database = GetDatabase();
            if (!database.Schemas.Contains(_VersionSchema))
            {
                var schema = new Schema(database, "version");
                schema.Create();
            }
            
            if (!database.Tables.Contains(_VersionTable, _VersionSchema))
            {

                var table = new Table(database, "Log", "version");


                var scriptColumnName = "Script";
                var deployDateColumnName = "DeployDate";
                var scriptColumn = new Column(table, scriptColumnName, DataType.NVarChar(2000));
                scriptColumn.Nullable = false;
                table.Columns.Add(scriptColumn);

                var deployDateColumn = new Column(table, deployDateColumnName, DataType.DateTimeOffset(7));
                deployDateColumn.Nullable = false;
                table.Columns.Add(deployDateColumn);

                table.Create();

                // Define Index object on the table by supplying the Table1 as the parent table and the primary key name in the constructor.  
                Index pk = new Index(table, $"{table.Schema}_{table.Name}_PK");
                pk.IndexKeyType = IndexKeyType.DriPrimaryKey;

                // Add Col1 as the Index Column  
                IndexedColumn idxCol1 = new IndexedColumn(pk, "Script");
                pk.IndexedColumns.Add(idxCol1);

                // Create the Primary Key  
                pk.Create();
            }
            
        }

        public Database GetDatabase()
        {
            if (Server.Databases.Contains(DatabaseName))
            {
                return Server.Databases[DatabaseName];
            }
            return null;
        }

        public void DropDatabase()
        {
            var database = GetDatabase();
            if (database != null)
            {
                database.Drop();
            }
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
