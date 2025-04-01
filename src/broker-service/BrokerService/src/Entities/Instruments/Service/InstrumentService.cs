using EasyTrade.BrokerService.Entities.Instruments.DTO;
using EasyTrade.BrokerService.Entities.Instruments.Repository;
using EasyTrade.BrokerService.Entities.Prices.ServiceConnector;
using EasyTrade.BrokerService.Entities.Products.Repository;
using EasyTrade.BrokerService.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EasyTrade.BrokerService.Entities.Instruments.Service;

public class InstrumentService(
    IInstrumentRepository instrumentRepository,
    IPriceServiceConnector priceServiceConnector,
    IProductRepository productRepository,
    ILogger<InstrumentService> logger
) : IInstrumentService
{
    private readonly IInstrumentRepository _instrumentRepository = instrumentRepository;
    private readonly IPriceServiceConnector _priceServiceConnector = priceServiceConnector;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ILogger _logger = logger;

    public async Task<IEnumerable<InstrumentDTO>> GetInstruments(int accountId)
    {
        _logger.LogInformation("Get instruments with account ID [{accountId}]", accountId);

        var instrumentDtoList = new List<InstrumentDTO>();

        _logger.LogInformation("Fetching all instruments from DB...");
        var instruments = await _instrumentRepository.GetAllInstruments().ToListAsync();
        _logger.LogInformation("Fetched {count} instruments", instruments.Count);

        _logger.LogInformation("Fetching owned instruments for account ID [{accountId}]", accountId);
        var ownedInstruments = await _instrumentRepository
            .GetOwnedInstrumentsOfAccount(accountId)
            .ToDictionaryAsync(x => x.InstrumentId, x => x);
        _logger.LogInformation("Fetched {count} owned instruments", ownedInstruments.Count);

        _logger.LogInformation("Calling PriceServiceConnector.GetLatestPrices()");
        var priceList = await _priceServiceConnector.GetLatestPrices();
        var prices = priceList.ToDictionary(x => x.InstrumentId, x => x);
        _logger.LogInformation("Fetched {count} prices", prices.Count);

        _logger.LogInformation("Fetching all products from DB...");
        var products = await _productRepository.GetProducts().ToDictionaryAsync(x => x.Id, x => x);
        _logger.LogInformation("Fetched {count} products", products.Count);

        foreach (var instrument in instruments)
        {
            if (!products.TryGetValue(instrument.ProductId, out var product))
            {
                _logger.LogError("Missing product for ProductId={productId} (InstrumentId={instrumentId})", instrument.ProductId, instrument.Id);
                continue;
            }

            if (!prices.TryGetValue(instrument.Id, out var price))
            {
                _logger.LogError("Missing price for InstrumentId={instrumentId}", instrument.Id);
                continue;
            }

            var ownedInstrument = ownedInstruments.TryGetValue(instrument.Id, out var value)
                ? value
                : default;

            var newInstrumentDto = new InstrumentDTO(instrument, ownedInstrument, product, price);
            instrumentDtoList.Add(newInstrumentDto);
        }

        _logger.LogInformation("Final instrument DTO count: {count}", instrumentDtoList.Count);
        _logger.LogDebug("Built instruments: {instruments}", instrumentDtoList.ToJson());

        return instrumentDtoList;
    }
}
