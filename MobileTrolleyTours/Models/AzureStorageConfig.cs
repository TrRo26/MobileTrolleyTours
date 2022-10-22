using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace MobileTrolleyTours.Models
{
    public class AzureStorageConfig
    {
        public string StorageAccountName
        {
            get
            {
                return "[REPLACE ME]";
            }
        }

        public string ConnectionString
        {
            get
            {
                return "[REPLACE ME]";
            }
        }

        public string TableTourScheduleChanges
        {
            get
            {
                return "[REPLACE ME]";
            }
        }
    }
}
