namespace bleak.Sql.VersionManager.Tests
{
    public class AppSettings
    {
        public DatabaseSettings Master { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    public class DatabaseSettings
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}