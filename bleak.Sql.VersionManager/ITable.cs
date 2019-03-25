using System.Collections.Generic;

namespace bleak.Sql.VersionManager
{
    public interface ITable
    {
        string Name { get; set; }
        IList<IColumn> Columns { get; set; }
    }

    public interface IColumn
    {
        string Name { get; set; }
    }
}