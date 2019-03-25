namespace bleak.Sql.VersionManager
{
    public static class StringExtensionMethods
    {
        public static string Minify(this string input)
        {
            return Minifier.Instance.SqlMinifier.Minify(input);
        }
    }
}