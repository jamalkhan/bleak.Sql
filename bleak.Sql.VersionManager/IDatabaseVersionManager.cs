using System;
using System.Collections.Generic;

namespace bleak.Sql.VersionManager
{
    public interface IDatabaseVersionManager : IDisposable
    {
        void IntializeDatabase();
        IDatabase CreateDatabase();
        void UpdateDatabase();
        void DropDatabase();
        IDatabase GetDatabase();
        IList<IVersionLog> GetDeployedChangesets();
    }
}