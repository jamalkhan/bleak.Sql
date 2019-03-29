using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace bleak.Sql.VersionManager.Redshift.Models.Database
{

    public class VersionManagerDbContext : DbContext
    {
        #region ConnectionString
        private object syncroot = new object();
        private string _connectionString;
        public string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {
                    lock (syncroot)
                    {
                        NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();
                        builder.Host = Host;
                        builder.Port = Port;
                        builder.Database = DatabaseName;
                        builder.Username = Username;
                        builder.Password = Password;
                        _connectionString = builder.ConnectionString;
                    }
                }
                return _connectionString;
            }
        }
        public string Host { get; private set; }
        public string DatabaseName { get; private set; }
        public int Port { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        #endregion ConnectionString

        #region Entity Framework
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RedshiftInformationSchemaColumn>()
                .HasKey(c => new { c.table_catalog, c.table_schema, c.table_name, c.column_name });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql(ConnectionString);
        #endregion Entity Framework

        #region Constructor
        public VersionManagerDbContext(string host, int port, string database, string username, string password) : base()
        {
            Host = host;
            Port = port;
            DatabaseName = database;
            Username = username;
            Password = password;
        }
        #endregion Constructor

        #region Properties
        public virtual DbSet<RedshiftInformationSchemaColumn> Columns { get; set; }
        public virtual DbSet<VersionLog> VersionLogs { get; set; }
        #endregion Properties
    }
}