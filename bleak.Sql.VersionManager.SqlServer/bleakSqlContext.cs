using Microsoft.EntityFrameworkCore;

namespace bleak.Sql.VersionManager
{
    public class VersionManagerDbContext : DbContext
    {
        #region Constructor
        public VersionManagerDbContext() : base() { }
        public VersionManagerDbContext(DbContextOptions<VersionManagerDbContext> options) : base(options) { }
        #endregion Constructor

        #region Properties
        public virtual DbSet<VersionLog> VersionLogs { get; set; }
        #endregion Properties
    }
}