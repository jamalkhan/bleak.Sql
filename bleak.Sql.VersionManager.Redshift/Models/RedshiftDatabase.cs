using System;
using System.Collections.Generic;

namespace bleak.Sql.VersionManager.Redshift.Models
{
    public class RedshiftDatabase : IDatabase
    {
        public string Name { get; set; }
        public IList<ISchema> Schemas { get; set; }
        public IList<ITable> Tables { get; set; }
        public DateTime CreateDate { get; set; }
        //internal Database SmoDatabase { get; set; }
        //internal Server SmoServer { get; set; }
        public void Drop()
        {
            try
            {
                //SmoServer.KillAllProcesses(Name);
                //SmoDatabase.Drop();
            }
            catch (Exception ex)
            {
                throw new RedshiftDropException();
            }
        }

        public void Backup()
        {
            //// Define a Backup object variable.   
            //Backup bk = new Backup();

            //// Specify the type of backup, the description, the name, and the database to be backed up.   
            //bk.Action = BackupActionType.Database;
            //bk.BackupSetDescription = $"Full backup of {Name}";
            //bk.BackupSetName = $"{Name} Backup";
            //bk.Database = $"{Name}";

            //// Declare a BackupDeviceItem by supplying the backup device file name in the constructor, and the type of device is a file.   
            //BackupDeviceItem bdi = default(BackupDeviceItem);
            //bdi = new BackupDeviceItem($"Full_Backup_{Name}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.bak", DeviceType.File);

            //// Add the device to the Backup object.   
            //bk.Devices.Add(bdi);
            //// Set the Incremental property to False to specify that this is a full database backup.   
            //bk.Incremental = false;

            //// Set the expiration date.   
            //bk.ExpirationDate = DateTime.Now.AddYears(1);

            //// Specify that the log must be truncated after the backup is complete.   
            //bk.LogTruncation = BackupTruncateLogType.Truncate;

            //// Run SqlBackup to perform the full database backup on the instance of SQL Server.
            //bk.SqlBackup(SmoServer);
        }
        //public string ServerAddress { get; set; }
        //public IEnumerable<ITable> Tables { get; set; }
    }
}