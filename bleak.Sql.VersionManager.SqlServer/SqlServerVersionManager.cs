using bleak.Sql.VersionManager.Models;
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
    public class SqlServerVersionManager : IDatabaseVersionManager
    {
        #region Properties
        private VersionManagerDbContext context;
        public const string _VersionSchema = "version";
        public const string _VersionTable = "log";
        public string Folder { get; protected set; }
        public string ServerInstance { get; private set; }
        public string DatabaseName { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public ServerConnection Connection { get; private set; }
        public Server Server { get; private set; }
        public IList<DdlScript> Scripts { get; set; } = new List<DdlScript>();
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

        private void IntializeDatabase()
        {
            var database = (SqlServerDatabase)GetDatabase();
            if (!database.Schemas.Any(s => s.Name == _VersionSchema))
            {
                var schema = new Schema(database.SmoDatabase, "version");
                schema.Create();
            }

            if (!database.Tables.Any(t => t.Name == _VersionTable && ((SqlServerTable)t).Schema == _VersionSchema))
            {
                var table = new Table(database.SmoDatabase, "Log", "version");

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
                foreach (string filename in Directory.GetFiles(sDir)
                    .Where(fn =>
                        Path.GetExtension(fn).ToLower() == ".sql"
                        )
                    .OrderBy(s => s))
                {
                    var extension = Path.GetExtension(filename);
                    if (!Scripts.Any(s => s.FileName == filename))
                    {
                        var script = new DdlScript();
                        script.Script = Path.GetFileName(filename);
                        script.FileName = filename;
                        Scripts.Add(script);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void ExecuteSql(string sql)
        {
            var database = new Database(Server, DatabaseName);
            database.ExecuteNonQuery(sql);
        }

        #endregion Internal Methods

        #region Database Management

        public IDatabase CreateDatabase()
        {
            var database = new Database(Server, DatabaseName);
            database.Create();
            IntializeDatabase();
            return GetDatabase();
        }

        public void DropDatabase(bool backup = true)
        {
            var database = (SqlServerDatabase)GetDatabase();
            if (database != null)
            {
                if (context != null)
                {
                    context.Dispose();
                }
                database.Drop(backup);
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
            DirSearch(Folder);
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