using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace bleak.Sql.VersionManager
{

    public class FileSystemScriptRepo: IScriptRepo
    {
        public FileSystemScriptRepo(string folder, string extension = ".sql")
        {
            Folder = folder;
            Extension = extension;
            LoadScripts();
        }
        public string Folder { get; protected set; }
        public string Extension { get; protected set; }
        public void Refresh()
        {
            LoadScripts();
        }
        void LoadScripts()
        {
            try
            {
                foreach (string filename in Directory.GetFiles(Folder)
                    .Where(fn =>
                        Path.GetExtension(fn).ToLower() == Extension
                        )
                    .OrderBy(s => s))
                {
                    var extension = Path.GetExtension(filename);
                    if (!Scripts.Any(s => s.FileName == filename))
                    {
                        var script = new ChangeScript();
                        script.Script = Path.GetFileName(filename);
                        script.FileName = filename;
                        Scripts.Add(script);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public IList<ChangeScript> Scripts { get; set; } = new List<ChangeScript>();
    }
}