using bleak.Sql.Minifier;

namespace bleak.Sql.VersionManager
{
    public class Minifier
    {
        #region Properties
        public SqlMinifier SqlMinifier { get; set; }
        private static object syncroot = new object();
        private static Minifier _instance;
        public static Minifier Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncroot)
                    {
                        if (_instance == null)
                        {
                            _instance = new Minifier();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion Properties

        #region Constructor
        private Minifier()
        {
            SqlMinifier = new SqlMinifier();
        }
        #endregion Constructor
    }
}