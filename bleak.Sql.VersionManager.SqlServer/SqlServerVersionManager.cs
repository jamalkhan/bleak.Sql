using bleak.Sql.VersionManager.SqlServer.Models;
using bleak.Sql.VersionManager.SqlServer.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DataType = Microsoft.SqlServer.Management.Smo.DataType;

namespace bleak.Sql.VersionManager.SqlServer
{
    public class SqlServerVersionManager : BaseDatabaseVersionManager, IDatabaseVersionManager
    {
        #region Properties
        private VersionManagerDbContext context;
        public const string _VersionSchema = "version";
        public const string _VersionTable = "log";
        public string ServerInstance { get; private set; }
        public string DatabaseName { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public ServerConnection Connection { get; private set; }
        public Server Server { get; private set; }
        public ILogger Logger { get; private set; }
        #endregion Properties

        #region Constructor
        public SqlServerVersionManager(
            string folder,
            string server,
            string username,
            string password,
            string databaseName,
            bool createDatabase = false,
            ILogger logger = null
            )
        {
            Folder = folder;
            Password = password;
            Username = username;
            ServerInstance = server;
            DatabaseName = databaseName;
            Connection = new ServerConnection(serverInstance: ServerInstance, userName: Username, password: Password);
            Server = new Server(Connection);
            Logger = logger;
            if (
                createDatabase
                && !string.IsNullOrEmpty(databaseName)
                && !Server.Databases.Contains(databaseName))
            {
                CreateDatabase();
            }
            IntializeDatabase();
            LoadScripts(Folder);

            if (!string.IsNullOrEmpty(databaseName))
            {
                var sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
                sqlConnectionStringBuilder.UserID = username;
                sqlConnectionStringBuilder.Password = password;
                sqlConnectionStringBuilder["Server"] = server;
                sqlConnectionStringBuilder.InitialCatalog = databaseName;
                var builder = new DbContextOptionsBuilder<VersionManagerDbContext>();
                builder.UseSqlServer(sqlConnectionStringBuilder.ConnectionString);
                context = new VersionManagerDbContext(builder.Options);
            }
        }
        #endregion Constructor

        #region IDisposable

        public void Dispose()
        {
            if (context != null)
            {
                context.Dispose();
            }
        }
        #endregion IDisposable

        #region Internal Methods

        public void IntializeDatabase()
        {
            if (Logger != null)
            {
                Logger.Log(LogLevel.Information, $"Initializing Database {DatabaseName}");
            }
            var database = (SqlServerDatabase)GetDatabase();
            if (!database.Schemas.Any(s => s.Name == _VersionSchema))
            {
                var schema = new Schema(database.SmoDatabase, "version");
                if (Logger != null)
                {
                    Logger.Log(LogLevel.Information, $"Creating version Schema in {DatabaseName}");
                }
                schema.Create();
                if (Logger != null)
                {
                    Logger.Log(LogLevel.Debug, $"Created version Schema in {DatabaseName}");
                }
            }

            if (!database.Tables.Any(t => t.Name == _VersionTable && ((SqlServerTable)t).Schema == _VersionSchema))
            {
                var table = new Table(database.SmoDatabase, "Log", "version");
                if (Logger != null)
                {
                    Logger.Log(LogLevel.Information, $"Creating version.Log Table in {DatabaseName}");
                }

                var scriptColumnName = "Script";
                var fileNameColumnName = "FileName";
                var deployDateColumnName = "DeployDate";

                var scriptColumn = new Column(table, scriptColumnName, DataType.NVarChar(2000));
                scriptColumn.Nullable = false;
                table.Columns.Add(scriptColumn);

                var fileNameColumn = new Column(table, fileNameColumnName, DataType.NVarCharMax);
                fileNameColumn.Nullable = false;
                table.Columns.Add(fileNameColumn);

                var deployDateColumn = new Column(table, deployDateColumnName, DataType.DateTimeOffset(7));
                deployDateColumn.Nullable = false;
                table.Columns.Add(deployDateColumn);

                table.Create();
                if (Logger != null)
                {
                    Logger.Log(LogLevel.Debug, $"Created version.Log Table in {DatabaseName}");
                }

                // Define Index object on the table by supplying the Table1 as the parent table and the primary key name in the constructor.  
                if (Logger != null)
                {
                    Logger.Log(LogLevel.Information, $"Creating {table.Schema}_{table.Name}_PK Primary Key in {DatabaseName}");
                }
                Index pk = new Index(table, $"{table.Schema}_{table.Name}_PK");
                pk.IndexKeyType = IndexKeyType.DriPrimaryKey;

                // Add Col1 as the Index Column  
                IndexedColumn idxCol1 = new IndexedColumn(pk, "Script");
                pk.IndexedColumns.Add(idxCol1);

                // Create the Primary Key  
                pk.Create();
                if (Logger != null)
                {
                    Logger.Log(LogLevel.Debug, $"Created {table.Schema}_{table.Name}_PK Primary Key in {DatabaseName}");
                }
            }
            if (Logger != null)
            {
                Logger.Log(LogLevel.Information, $"Database {DatabaseName} has been initialized.");
            }
        }



        private void ExecuteSql(string sql)
        {
            if (Logger != null)
            {
                Logger.Log(LogLevel.Information, $"Executing Script against {DatabaseName}.");
                Logger.Log(LogLevel.Debug, $"{sql}");
            }
            // TODO: why am I defining a database everytime?!
            var database = new Database(Server, DatabaseName);
            database.ExecuteNonQuery(sql);
            if (Logger != null)
            {
                Logger.Log(LogLevel.Information, $"Executed Script against {DatabaseName}.");
            }
        }

        #endregion Internal Methods

        #region Database Management

        public IDatabase CreateDatabase()
        {
            // TODO: What happens if the database already exists?!

            var database = new Database(Server, DatabaseName);
            database.Create();
            return GetDatabase();
        }

        public void DropDatabase()
        {
            var database = (SqlServerDatabase)GetDatabase();
            if (database != null)
            {
                if (context != null)
                {
                    context.Dispose();
                }
                database.Drop();
            }
        }

        public IDatabase GetDatabase()
        {
            if (Server.Databases.Contains(DatabaseName))
            {
                var smoDatabase = Server.Databases[DatabaseName];
                return smoDatabase.ConvertToIDatabase(server: Server);
            }
            return null;
        }


        #endregion Database Management

        #region Version Management

        public void UpdateDatabase()
        {
            LoadScripts(Folder);
            foreach (var script in Scripts.OrderBy(s => s.Script))
            {
                if (context.VersionLogs.Count(vl => vl.Script == script.Script) == 0)
                {
                    var sql = script.LoadFullText();
                    ExecuteSql(sql);
                    VersionLog log = new VersionLog();
                    log.Script = script.Script;
                    log.FileName = script.FileName;
                    log.DeployDate = DateTimeOffset.Now;
                    context.VersionLogs.Add(log);
                }
            }
            context.SaveChanges();
        }

        public IList<IVersionLog> GetDeployedChangesets()
        {
            var versionLogs = context.VersionLogs;
            var retval = new List<IVersionLog>();
            foreach (var versionLog in versionLogs)
            {
                retval.Add(versionLog);
            }
            return retval;
        }

        #endregion Version Management
    }
}