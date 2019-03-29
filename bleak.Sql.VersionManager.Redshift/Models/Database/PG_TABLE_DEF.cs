using System.ComponentModel.DataAnnotations.Schema;

namespace bleak.Sql.VersionManager.Redshift.Models.Database
{

    [Table("PG_TABLE_DEF")]
    public class PG_TABLE_DEF
    {
        public string schemaname { get; set; }
        public string tablename { get; set; }
        public string column { get; set; }
        public string type { get; set; }
        public string encoding { get; set; }
        public bool distkey { get; set; }
        public int sortkey { get; set; }
        public bool notnull { get; set; }
    }
}