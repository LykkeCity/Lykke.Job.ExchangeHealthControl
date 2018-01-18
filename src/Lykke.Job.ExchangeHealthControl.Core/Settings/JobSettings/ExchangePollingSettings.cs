namespace Lykke.Job.ExchangeHealthControl.Core.Settings.JobSettings
{
    public class PollingSettings
    {
        public string[] ExchangeList { get; set; }
        
        public int PollingPeriodMilliseconds { get; set; }
    }
}
