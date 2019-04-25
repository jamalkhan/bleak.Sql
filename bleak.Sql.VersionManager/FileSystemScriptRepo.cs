using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace bleak.Sql.VersionManager
{
    public class FileSystemScriptRepo : IScriptRepo
    {
        public FileSystemScriptRepo(string folder, string extension = ".sql")
        {
            Folder = folder;
            Extension = extension;
            LoadScripts();
        }

        public FileSystemScriptRepo(ILogger logger, string folder, string extension = ".sql") : this(folder, extension)
        {
            Logger = logger;
        }
        public ILogger Logger { get; protected set; }
        public string Folder { get; protected set; }
        public string Extension { get; protected set; }
        public void Refresh()
        {
            LoadScripts();
        }

        private void LoadScripts()
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
                        if (Logger != null)
                        {
                            Logger.Log(LogLevel.Debug, $"Adding {filename} to the list of known files");
                        }
                        var script = new ChangeScript();
                        script.Script = Path.GetFileName(filename);
                        script.FileName = filename;
                        Scripts.Add(script);
                    }
                }
            }
            catch (Exception ex)
            {
                if (Logger != null)
                {
                    Logger.Log(LogLevel.Error, $"Error Cataloging Files: {ex.Message}");
                }
                
            }
        }

        public IList<ChangeScript> Scripts { get; set; } = new List<ChangeScript>();
    }
}