using bleak.Sql.VersionManager.Redshift.Models.Database;
using System.Collections.Generic;
using System.Linq;

namespace bleak.Sql.VersionManager.Redshift.Models
{
    public class RedshiftColumn : IColumn
    {
        public string Name { get; set; }
    }

    public static class RedshiftColumnExtensionMethods
    {
        public static RedshiftColumn ConvertToRedshiftColumn(this RedshiftInformationSchemaColumn colum)
        {
            RedshiftColumn retval = new RedshiftColumn();
            retval.Name = colum.column_name;
            return retval;
        }
        public static RedshiftDatabase ConvertToRedshiftDatabase(this IEnumerable<RedshiftInformationSchemaColumn> columns)
        {
            RedshiftDatabase database = new RedshiftDatabase();
            database.Schemas = columns
                .Select(sch => sch.table_schema)
                .Distinct()
                .Select(s => new RedshiftSchema() { Name = s })
                .Cast<ISchema>()
                .ToList();
            database.Tables = columns
                .Select(s => new RedshiftTable() { Schema = s.table_schema, Name = s.table_name })
                .Distinct()
                .Cast<ITable>()
                .ToList();

            foreach (var table in database.Tables.Cast<RedshiftTable>())
            {
                table.Columns = new List<IColumn>();
                foreach (var cx in columns
                    .Where(c => c.table_name == table.Name && c.table_schema == table.Schema)
                    .OrderBy(c => c.ordinal_position))
                {
                    var column = ConvertToRedshiftColumn(cx);
                    table.Columns.Add(column);
                }
            }
            return database;
        }
    }
}