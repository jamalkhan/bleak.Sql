# bleak.Sql

Currently deployed:
https://www.nuget.org/packages/bleak.Sql.Minifier/
https://www.nuget.org/packages/bleak.Sql.VersionManager/


Usage
```
  private static void UpgradeDatabase(string connectionString, string folder)
        {
            var decoder = new SqlConnectionStringBuilder(connectionString);

            Console.WriteLine($"Connecting to [{decoder["Server"]}].[{decoder.InitialCatalog}]. UserId = {decoder.UserID}; Password = {decoder.Password.Substring(0,2)}************, DatabaseName = {decoder.InitialCatalog}, Server = {decoder.DataSource}");
            var versionManager = new SqlServerVersionManager(
                folder: folder,
                server: decoder.DataSource,
                username: decoder.UserID,
                password: decoder.Password,
                databaseName: decoder.InitialCatalog,
                createDatabase: true
                );

            Console.WriteLine($"{versionManager.Scripts.Count} Scripts were loaded");

            try
            {
                versionManager.IntializeDatabase();
                Console.WriteLine($"Database has been initialized");
            }
            catch (Exception)
            {
                Console.WriteLine($"Database may already be initialized");
            }

            versionManager.UpdateDatabase();
            Console.WriteLine($"Database {decoder.InitialCatalog} has been upgraded.");
        }
```
