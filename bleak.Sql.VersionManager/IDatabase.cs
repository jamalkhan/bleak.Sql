namespace bleak.Sql.VersionManager
{

    public interface IDatabase
    {
        string Name { get; set; }
        void Drop();
        //string ServerAddress { get; set; }

        //IEnumerable<ITable> Tables { get; set; }
    }
}