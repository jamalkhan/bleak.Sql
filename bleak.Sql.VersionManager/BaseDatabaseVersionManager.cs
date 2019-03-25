using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace bleak.Sql.VersionManager
{

    public class BaseDatabaseVersionManager
    {
        public IList<DdlScript> Scripts { get; set; } = new List<DdlScript>();
        public string Folder { get; protected set; }
        public virtual string ScriptExtension { get; set; } = ".sql";
        public void LoadScripts(string sDir)
        {
            try
            {
                foreach (string filename in Directory.GetFiles(sDir)
                    .Where(fn =>
                        Path.GetExtension(fn).ToLower() == ScriptExtension
                        )
                    .OrderBy(s => s))
                {
                    var extension = Path.GetExtension(filename);
                    if (!Scripts.Any(s => s.FileName == filename))
                    {
                        var script = new DdlScript();
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
    }
}