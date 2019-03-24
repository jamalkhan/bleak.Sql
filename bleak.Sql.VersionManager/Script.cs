using System.IO;

namespace bleak.Sql.VersionManager
{
    public class Script
    {
        public int Index { get; set; }
        public string FileName { get; set; }
        public string LoadFullText(bool minify = true)
        {
            string data = File.ReadAllText(FileName);
            if (minify)
            {
                return data.Minify();
            }
            return data;
        }
    }
}