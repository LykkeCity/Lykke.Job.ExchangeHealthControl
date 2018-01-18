using Lykke.Job.ExchangeHealthControl.Core.Settings.JobSettings;
using Lykke.Job.ExchangeHealthControl.Core.Settings.SlackNotifications;

namespace Lykke.Job.ExchangeHealthControl.Core.Settings
{
    public class ExchangeHealthControlSettings
    {
        public ExchangeHealthControlJobSettings ExchangeHealthControlJob { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
