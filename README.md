# Exchange Health Control Job

Stateful exchange polling job supplements Exchange Connector.

Service polls the list of exchanges to:
1. Get IsAlive status from all the exchanges. Successful position receiving may be accounted as IsAlive.
Write fail/restore events to the rabbit for consumers (HedgingSystem, MarketMaker).
2. Account performance metrics that depends on current/average responce speed, fail frequency.
Write these metrics to the rabbit.