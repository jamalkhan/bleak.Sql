using Microsoft.EntityFrameworkCore;

namespace bleak.Sql.VersionManager
{

    public class bleakSqlContext : DbContext
    {
        #region Constructor
        public bleakSqlContext() : base() { }
        public bleakSqlContext(DbContextOptions<bleakSqlContext> options) : base(options) { }
        #endregion Constructor

        #region Properties
        public virtual DbSet<VersionLog> VersionLogs { get; set; }
        #endregion Properties
    }
}