using System.Collections.Generic;

namespace bleak.Sql.VersionManager
{

    public interface IScriptRepo
    {
        IList<ChangeScript> Scripts { get; set; }
        void Refresh();
    }
}