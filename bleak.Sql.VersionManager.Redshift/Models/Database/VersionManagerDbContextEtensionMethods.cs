using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace bleak.Sql.VersionManager.Redshift.Models.Database
{
    
    public static class VersionManagerDbContextEtensionMethods
    {
        public static int ExecuteNonQuery(this VersionManagerDbContext context, string sql, params SqlCommandParameter[] parameters)
        {
            var connection = (NpgsqlConnection)context.Database.GetDbConnection();
            using (var command = new NpgsqlCommand(sql, connection))
            {
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param.ConvertToNpgsqlCommand());
                }
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }
                return command.ExecuteNonQuery();
            }
        }

        public static NpgsqlParameter ConvertToNpgsqlCommand( this SqlCommandParameter parameter)
        {
            NpgsqlParameter retval= new NpgsqlParameter();
            retval.ParameterName = parameter.Name;
            retval.Value = parameter.Value;
            return retval;
        }
    }
}