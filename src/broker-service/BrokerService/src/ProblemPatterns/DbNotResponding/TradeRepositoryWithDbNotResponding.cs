using EasyTrade.BrokerService.Helpers;
using EasyTrade.BrokerService.ProblemPatterns.OpenFeature;
using Microsoft.Extensions.Logging.Abstractions;

namespace EasyTrade.BrokerService.Entities.Trades.Repository;

public class TradeRepositoryWithDbNotResponding(
    BrokerDbContext dbContext,
    IPluginManager pluginManager
) : TradeRepository(dbContext, NullLogger<TradeRepository>.Instance)
{
    private readonly IPluginManager _pluginManager = pluginManager;

    public override void AddTrade(Trade trade)
    {
        if (CheckIfProblemPatternIsOn())
        {
            // trade.Id = Constants.InvalidTradeId;
            trade.Id = 99999999;
        }
        base.AddTrade(trade);
    }

    private bool CheckIfProblemPatternIsOn()
    {
        var task = Task.Run(
            async () => await _pluginManager.GetPluginState(Constants.DbNotResponding, false)
        );
        return task.Result;
    }
}
