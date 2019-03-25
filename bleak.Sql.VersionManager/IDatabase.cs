namespace bleak.Sql.VersionManager
{

    public interface IDatabase
    {
        string Name { get; set; }
        void Drop(bool backup = true);
        //string ServerAddress { get; set; }

        //IEnumerable<ITable> Tables { get; set; }
    }
}