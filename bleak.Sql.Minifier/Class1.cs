using System;

namespace bleak.Sql.Minifier
{
    public class SqlMinifier
    {
        public string Minify(string sql)
        {
            return sql.Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("  ", " ");
        }
    }
}
