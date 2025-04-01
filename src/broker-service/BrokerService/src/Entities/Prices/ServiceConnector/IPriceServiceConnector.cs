using EasyTrade.BrokerService.Entities.Prices; // For Price class
using EasyTrade.BrokerService.Entities.Prices.ServiceConnector; // For IPriceServiceConnector interface

namespace EasyTrade.BrokerService.Entities.Prices.ServiceConnector;

public interface IPriceServiceConnector
{
    public Task<IEnumerable<Price>> GetPricesByInstrumentId(int id, int count);
    public Task<IEnumerable<Price>> GetLatestPrices();
    public Task<Price?> GetLastPriceByInstrumentId(int id);
}
