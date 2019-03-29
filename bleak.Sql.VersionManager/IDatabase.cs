using System.Collections.Generic;

namespace bleak.Sql.VersionManager
{
    public interface IDatabase
    {
        string Name { get; set; }
        IList<ITable> Tables { get; set; }
        void Drop();
    }
}