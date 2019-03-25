using System;
using System.Collections.Generic;

namespace bleak.Sql.VersionManager
{
    public interface IDatabaseVersionManager: IDisposable
    {
        IDatabase CreateDatabase();
        void DropDatabase(bool backup = true);
        IDatabase GetDatabase();
        IList<IVersionLog> GetDeployedChangesets();
        void UpdateDatabase();
    }
}