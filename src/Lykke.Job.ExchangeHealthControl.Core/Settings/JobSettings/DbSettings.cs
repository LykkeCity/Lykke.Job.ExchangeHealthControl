namespace Lykke.Job.ExchangeHealthControl.Core.Settings.JobSettings
{
    public class DbSettings
    {
        public string StorageConnString { get; set; }
        public string LogsConnString { get; set; }
        
        public AzureTablesSettings Tables { get; set; }
    }
}
