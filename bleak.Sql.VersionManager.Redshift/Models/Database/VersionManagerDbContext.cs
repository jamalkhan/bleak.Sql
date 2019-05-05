using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace bleak.Sql.VersionManager.Redshift.Models.Database
{
    public enum SslMode
    {
        Require,
        Prefer,
        Disable,
    }
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
                        builder.ServerCompatibilityMode = CompatibilityMode;
                        if (SslMode.HasValue)
                        {
                            switch (SslMode.Value)
                            {
                                case Models.Database.SslMode.Prefer:
                                    builder.SslMode = Npgsql.SslMode.Prefer;
                                    break;
                                case Models.Database.SslMode.Disable:
                                    builder.SslMode = Npgsql.SslMode.Disable;
                                    break;
                                case Models.Database.SslMode.Require:
                                    builder.SslMode = Npgsql.SslMode.Require;
                                    break;
                            }
                        }
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
        public SslMode? SslMode { get; private set; }
        public ServerCompatibilityMode CompatibilityMode { get; private set; }
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
        public VersionManagerDbContext(string host, int port, string database, string username, string password, SslMode? sslMode = null) : base()
        {
            Host = host;
            Port = port;
            DatabaseName = database;
            Username = username;
            Password = password;
            SslMode = sslMode;
            // TODO: This can be made configurable if targeting Postgres?
            // Requires testing.
            CompatibilityMode = ServerCompatibilityMode.Redshift;
        }
        #endregion Constructor

        #region Properties
        public virtual DbSet<RedshiftInformationSchemaColumn> Columns { get; set; }
        public virtual DbSet<VersionLog> VersionLogs { get; set; }
        #endregion Properties
    }
}