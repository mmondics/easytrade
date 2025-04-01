using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EasyTrade.BrokerService.Helpers;

namespace EasyTrade.BrokerService.Entities.Trades;

public class Trade
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int AccountId { get; set; }
    public int InstrumentId { get; set; }
    public string Direction { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal EntryPrice { get; set; }
    public DateTime TimestampOpen { get; set; }
    public DateTime? TimestampClose { get; set; }
    public bool TradeClosed { get; set; }
    public bool TransactionHappened { get; set; }
    public string Status { get; set; } = string.Empty;

    public Trade() { }

    public Trade(
        int accountId,
        int instrumentId,
        string direction,
        decimal quantity,
        decimal entryPrice,
        DateTime timestampOpen,
        DateTime? timestampClose,
        bool tradeClosed,
        bool transactionHappened,
        string status
    )
    {
        AccountId = accountId;
        InstrumentId = instrumentId;
        Direction = direction;
        Quantity = quantity;
        EntryPrice = entryPrice;
        TimestampOpen = timestampOpen;
        TimestampClose = timestampClose;
        TradeClosed = tradeClosed;
        TransactionHappened = transactionHappened;
        Status = status;
    }

    public static Trade QuickTrade(
        int accountId,
        int instrumentId,
        ActionType direction,
        decimal price,
        decimal quantity
    ) =>
        new(
            accountId,
            instrumentId,
            direction.ToString().ToLower(),
            quantity,
            price,
            DateTime.UtcNow,
            DateTime.UtcNow,
            true,
            true,
            "Instant " + direction + " done."
        );

    public static Trade LongTrade(
        int accountId,
        int instrumentId,
        ActionType direction,
        decimal price,
        decimal quantity,
        int duration
    ) =>
        new(
            accountId,
            instrumentId,
            direction.ToString().ToLower(),
            quantity,
            price,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(duration),
            false,
            false,
            direction + " registered."
        );
}
