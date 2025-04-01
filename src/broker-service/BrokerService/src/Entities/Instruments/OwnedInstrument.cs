using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyTrade.BrokerService.Entities.Instruments;

[Table("Ownedinstruments")]
public class OwnedInstrument
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int AccountId { get; set; }
    public int InstrumentId { get; set; }
    public decimal Quantity { get; set; }
    public DateTime LastModificationDate { get; set; }

    public OwnedInstrument() { }

    public OwnedInstrument(int accountId, int instrumentId, decimal quantity, DateTime lastModificationDate)
    {
        AccountId = accountId;
        InstrumentId = instrumentId;
        Quantity = quantity;
        LastModificationDate = lastModificationDate;
    }

    public OwnedInstrument(int accountId, int instrumentId, decimal quantity)
        : this(accountId, instrumentId, quantity, DateTime.UtcNow) { }
}
