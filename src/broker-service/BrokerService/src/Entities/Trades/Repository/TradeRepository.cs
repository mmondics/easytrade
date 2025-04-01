using EasyTrade.BrokerService.Helpers;

namespace EasyTrade.BrokerService.Entities.Trades.Repository;

public class TradeRepository : TransactionalRepository, ITradeRepository
{
    private readonly ILogger<TradeRepository> _logger;

    public TradeRepository(BrokerDbContext dbContext, ILogger<TradeRepository> logger)
        : base(dbContext)
    {
        _logger = logger;
    }

    public virtual void AddTrade(Trade trade) => DbContext.Trades.Add(trade);

    public IQueryable<Trade> GetAllTrades() => DbContext.Trades.AsQueryable();

    public void UpdateTrade(Trade trade) => DbContext.Trades.Update(trade);
}
