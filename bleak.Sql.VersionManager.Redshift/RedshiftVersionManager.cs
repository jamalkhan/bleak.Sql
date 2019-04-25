using bleak.Sql.VersionManager.Redshift.Models;
using bleak.Sql.VersionManager.Redshift.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace bleak.Sql.VersionManager.Redshift
{
    public class RedshiftVersionManager : IDatabaseVersionManager
    {
        #region Constants
        private const string _VersionSchema = "version";
        private const string _VersionTable = "log";
        private const string _ScriptCreateVersionLogTable = "create table if not exists version.log(script nvarchar(1000) not null,filename nvarchar(1000) not null,deploydate TIMESTAMPTZ not null,primary key(script))distkey(deploydate)compound sortkey(script, deploydate);";
        private const string _ScriptCreateVersionSchema = "create schema if not exists version;";
        #endregion Constants

        #region Properties
        public string Host { get; private set; }
        public int Port { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Database { get; private set; }
        public VersionManagerDbContext DbContext { get; private set; }
        public IScriptRepo ScriptRepo { get; private set; }
        #endregion Properties

        public RedshiftVersionManager(
            IScriptRepo scriptRepo,
            string host,
            int port,
            string database,
            string username,
            string password)
        {
            ScriptRepo = scriptRepo;
            Host = host;
            Port = port;
            Database = database;
            Username = username;
            Password = password;
            DbContext = new VersionManagerDbContext(Host, Port, Database, Username, Password);
        }

        public IDatabase CreateDatabase()
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void DropDatabase()
        {
            throw new System.NotImplementedException();
        }

        public IDatabase GetDatabase()
        {
            var columns = DbContext.Columns.ToList();
            return columns.ConvertToRedshiftDatabase();
        }

        public IList<IVersionLog> GetDeployedChangesets()
        {
            throw new System.NotImplementedException();
        }

        public void IntializeDatabase()
        {
            var database = (RedshiftDatabase)GetDatabase();
            if (!database.Schemas.Any(s => s.Name == _VersionSchema))
            {
                DbContext.ExecuteNonQuery(_ScriptCreateVersionSchema);
            }

            DbContext.ExecuteNonQuery(_ScriptCreateVersionLogTable);
        }

        public void UpdateDatabase()
        {
            ScriptRepo.Refresh();
            var versionLogs = DbContext.VersionLogs.ToList();
            foreach (var script in ScriptRepo.Scripts.OrderBy(s=> s.Script))
            {
                if (versionLogs.Count(vl => vl.Script == script.Script) == 0)
                {
                    var changeScript = script.LoadFullText();
                    DbContext.ExecuteNonQuery(changeScript);
                    VersionLog log = new VersionLog();
                    log.Script = script.Script;
                    log.FileName = script.FileName;
                    log.DeployDate = DateTimeOffset.Now;
                    DbContext.VersionLogs.Add(log);
                }
            }
            DbContext.SaveChanges();
        }
    }
}