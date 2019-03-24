using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using DataType = Microsoft.SqlServer.Management.Smo.DataType;

namespace bleak.Sql.VersionManager
{
    public class SqlServerVersionManager
    {
        #region Properties
        private bleakSqlContext context;
        public const string _VersionSchema = "version";
        public const string _VersionTable = "log";
        public string Folder { get; private set; }
        public string ServerInstance { get; private set; }
        public string DatabaseName { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public ServerConnection Connection { get; private set; }
        public Server Server { get; private set; }
        public IList<Script> Scripts { get; set; } = new List<Script>();
        #endregion Properties

        #region Constructor
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

            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
            sqlConnectionStringBuilder.UserID = username;
            sqlConnectionStringBuilder.Password = password;
            sqlConnectionStringBuilder["Server"] = server;
            sqlConnectionStringBuilder.InitialCatalog = databaseName;
            var builder = new DbContextOptionsBuilder<bleakSqlContext>();
            builder.UseSqlServer(sqlConnectionStringBuilder.ConnectionString);
            context = new bleakSqlContext(builder.Options);
        }
        #endregion Constructor

        #region Internal Methods

        private void IntializeDatabase()
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

        private void ExecuteSql(string sql)
        {
            var database = new Database(Server, DatabaseName);
            database.ExecuteNonQuery(sql);
        }

        #endregion Internal Methods

        #region Database Management

        public Database CreateDatabase()
        {
            var database = new Database(Server, DatabaseName);
            database.Create();
            IntializeDatabase();
            return GetDatabase();
        }

        public void DropDatabase()
        {
            var database = GetDatabase();
            if (database != null)
            {
                database.Drop();
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

        #endregion Database Management

        #region Version Management

        public void UpdateDatabase()
        {
            foreach (var script in Scripts.OrderBy(s => s.FileName))
            {
                var sql = script.LoadFullText();
                ExecuteSql(sql);
                VersionLog log = new VersionLog();
                log.Script = script.FileName;
                log.DeployDate = DateTimeOffset.Now;
                context.VersionLogs.Add(log);
            }
            context.SaveChanges();
        }

        #endregion Version Management
    }
}