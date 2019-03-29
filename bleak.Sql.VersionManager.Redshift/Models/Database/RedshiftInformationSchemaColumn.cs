using System.ComponentModel.DataAnnotations.Schema;

namespace bleak.Sql.VersionManager.Redshift.Models.Database
{

    [Table(name:"columns",Schema= "information_schema")]
    public class RedshiftInformationSchemaColumn
    {
        public string table_catalog { get; set; }
        public string table_schema { get; set; }
        public string table_name { get; set; }
        public string column_name { get; set; }
        public int? ordinal_position { get; set; }
        //public string column_default { get; set; }
        //public string is_nullable { get; set; }
        //public string data_type { get; set; }
        //public int? character_maximum_length { get; set; }
        //public int character_octet_length { get; set; }
        //public int numeric_precision { get; set; }
        //public int numeric_precision_radix { get; set; }
        //public int numeric_scale { get; set; }
        //public int datetime_precision { get; set; }
        //public int interval_type { get; set; }
        //public int interval_precision { get; set; }
        //public int interval_precision { get; set; }
    }
}