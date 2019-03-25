using System.Collections.Generic;

namespace bleak.Sql.VersionManager.Models
{
    public class SqlServerTable : ITable
    {
        public string Name { get; set; }
        public string Schema { get; set; }
        public IList<IColumn> Columns { get; set; }
    }
}