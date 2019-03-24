using Microsoft.SqlServer.Management.Smo;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bleak.Sql.VersionManager
{

    [Table("Log", Schema = "version")]
    public class VersionLog
    {
        [Key]
        public string Script { get; set; }
        public DateTimeOffset DeployDate { get; set; }
    }
}