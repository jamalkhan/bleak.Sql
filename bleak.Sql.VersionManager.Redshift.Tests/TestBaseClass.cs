using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace bleak.Sql.VersionManager.Redshift.Tests
{

    public abstract class TestBaseClass
    {
        protected RedshiftVersionManager versionManager;
        protected string Folder = Path.Combine(Directory.GetCurrentDirectory(), "Scripts");

        [TestCleanup]
        public virtual void Dispose()
        {
        }
        [TestInitialize]
        public virtual void Initialize()
        {

        }


        protected TestBaseClass()
        {
            versionManager = new RedshiftVersionManager(
                scriptRepo: new FileSystemScriptRepo(Folder),
                host: Configuration.Settings.Master.Host,
                port: Configuration.Settings.Master.Port,
                database: Configuration.Settings.Master.Database,
                username: Configuration.Settings.Master.Username,
                password: Configuration.Settings.Master.Password);
        }
    }
}