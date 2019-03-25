using Microsoft.SqlServer.Management.Smo;
using System.Collections.Generic;

namespace bleak.Sql.VersionManager.Models
{
    public static class SqlServerDatabaseExtensionMethods
    {
        public static IDatabase ConvertToIDatabase(this Database database, Server server)
        {
            var retval = new SqlServerDatabase();
            retval.Name = database.Name;
            retval.CreateDate = database.CreateDate;
            retval.Schemas = new List<ISchema>();
            retval.SmoDatabase = database;
            retval.SmoServer = server;
            foreach (Schema schema in database.Schemas)
            {
                var sqlServerSchema = new SqlServerSchema();
                sqlServerSchema.Name = schema.Name;
                retval.Schemas.Add(sqlServerSchema);
            }
            retval.Tables = new List<ITable>();
            database.Tables.Refresh();
            foreach (Table table in database.Tables)
            {
                var sqlServerTable = new SqlServerTable();
                sqlServerTable.Name = table.Name;
                sqlServerTable.Schema = table.Schema;
                sqlServerTable.Columns = new List<IColumn>();
                table.Refresh();
                foreach (Column column in table.Columns)
                {
                    SqlServerColumn sqlColumn = new SqlServerColumn();
                    sqlColumn.Name = column.Name;
                    sqlServerTable.Columns.Add(sqlColumn);
                }

                retval.Tables.Add(sqlServerTable);
            }
            return retval;
        }
    }
}