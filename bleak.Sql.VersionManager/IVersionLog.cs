using System;

namespace bleak.Sql.VersionManager
{
    public interface IVersionLog
    {
        DateTimeOffset DeployDate { get; set; }
        string FileName { get; set; }
        string Script { get; set; }
    }
}