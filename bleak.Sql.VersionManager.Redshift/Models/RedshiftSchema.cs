namespace bleak.Sql.VersionManager.Redshift.Models
{
    public class RedshiftSchema : ISchema
    {
        public string Name { get; set; }
        public override bool Equals(object obj)
        {
            if (obj is RedshiftSchema)
            {
                var robj = (RedshiftSchema)obj;
                return robj.Name == Name;
            }
            return false;
        }
        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Name.GetHashCode();
            return hash;
        }
    }
}