using Microsoft.Extensions.Logging;

namespace bleak.Sql.VersionManager
{

    public static class StringExtensionMethods
    {
        public static string Minify(this string input, ILogger logger = null)
        {
            if (logger != null)
            {
                logger.Log(LogLevel.Trace, $"Minifying Sql. Input: {input}");
            }
            var retval = Minifier.Instance.SqlMinifier.Minify(input);
            if (logger != null)
            {
                logger.Log(LogLevel.Trace, $"Minifying Sql. Output: {retval}");
            }
            return retval;
        }
    }
}