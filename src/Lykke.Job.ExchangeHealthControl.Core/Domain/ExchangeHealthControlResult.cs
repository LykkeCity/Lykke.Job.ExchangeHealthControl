using System;

namespace Lykke.Job.ExchangeHealthControl.Core.Domain
{
    public class ExchangeHealthControlResult
    {
        public string ExchangeName { get; private set; }
        public int RequestDurationMilliseconds { get; private set; }
        public string Type { get; private set; }
        public Exception Exception { get; private set; }
        public bool IsSuccessful { get; private set; }

        public ExchangeHealthControlResult(){}
        
        public ExchangeHealthControlResult(string exchangeName, int requestDurationMilliseconds, string type,
            Exception exception, bool isSuccessful)
        {
            ExchangeName = exchangeName;
            RequestDurationMilliseconds = requestDurationMilliseconds;
            Type = type;
            Exception = exception;
            IsSuccessful = isSuccessful;
        }

        public override string ToString()
        {
            return "Exchange: " + ExchangeName + ", "
                   + "Request duration ms: " + RequestDurationMilliseconds + ", "
                   + "Type: " + Type + ", "
                   + "IsSuccessful: " + IsSuccessful + ", ";
        }
    }
}
