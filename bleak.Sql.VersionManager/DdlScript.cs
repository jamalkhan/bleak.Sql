using System.IO;

namespace bleak.Sql.VersionManager
{

    public class ChangeScript
    {
        public int Index { get; set; }
        public string Script { get; set; }
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