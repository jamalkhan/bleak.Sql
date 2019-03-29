using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bleak.Sql.VersionManager.Redshift.Models.Database
{
    [Table("Log", Schema = "version")]
    public class VersionLog : IVersionLog
    {
        [Key]
        public string Script { get; set; }
        public string FileName { get; set; }
        public DateTimeOffset DeployDate { get; set; }
    }
}
