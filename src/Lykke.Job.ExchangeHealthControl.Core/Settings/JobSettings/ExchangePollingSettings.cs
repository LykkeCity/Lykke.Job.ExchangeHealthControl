namespace Lykke.Job.ExchangeHealthControl.Core.Settings.JobSettings
{
    public class ExchangePollingSettings
    {
        public string[] ExchangeList { get; set; }
        
        public int PollingPeriodMilliseconds { get; set; }
    }
}
