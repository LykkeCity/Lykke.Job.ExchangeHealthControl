namespace Lykke.Job.ExchangeHealthControl.Core.Settings.JobSettings
{
    public class ExchangeHealthControlJobSettings
    {
        public DbSettings Db { get; set; }

        public RabbitMqSettings Rabbit { get; set; }
        
        public ServicesSettings Services { get; set; }
        
        public ExchangeSettings JfdSettings { get; set; }
        
        public ExchangeSettings IcmSettings { get; set; }
        
        public int DataSavingPeriodMilliseconds { get; set; }
        
        public int QuotesTtlMilliseconds { get; set; }
    }
}
