using System.Collections.Generic;

namespace bleak.Sql.VersionManager.Redshift.Models
{
    public class RedshiftTable : ITable
    {
        public string Name { get; set; }
        public string Schema { get; set; }
        public IList<IColumn> Columns { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is RedshiftTable)
            {
                var robj = (RedshiftTable)obj;
                return robj.Name == Name && robj.Schema == Schema;
            }
            return false;
        }
        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Name.GetHashCode();
            hash = (hash * 7) + Schema.GetHashCode();
            return hash;
        }
        public override string ToString()
        {
            return $"{Schema}.{Name}";
        }
    }
}