﻿using System.Collections.Generic;
using Lykke.Job.ExchangeHealthControl.Core.Domain.Health;

namespace Lykke.Job.ExchangeHealthControl.Core.Services
{
    // NOTE: See https://lykkex.atlassian.net/wiki/spaces/LKEWALLET/pages/35755585/Add+your+app+to+Monitoring
    public interface IHealthService
    {
        string GetHealthViolationMessage();
        IEnumerable<HealthIssue> GetHealthIssues();

        // TODO: Place health tracing methods declarations here
    }
}
