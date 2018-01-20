using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.ExchangeHealthControl.Core.Settings.JobSettings
{
    public class AzureTableConnectionParams
    {
        [Optional]
        public string ConnectionString { get; set; }

        public string TableName { get; set; }
    }
}
