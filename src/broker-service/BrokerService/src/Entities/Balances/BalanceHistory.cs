using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EasyTrade.BrokerService.Helpers;

namespace EasyTrade.BrokerService.Entities.Balances;

[Table("Balancehistory")]
public class BalanceHistory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int AccountId { get; set; }
    public decimal OldValue { get; set; }
    public decimal ValueChange { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public DateTime ActionDate { get; set; }

    public BalanceHistory() { }

    public BalanceHistory(
        int accountId,
        decimal oldValue,
        decimal valueChange,
        string actionType,
        DateTime actionDate
    )
    {
        AccountId = accountId;
        OldValue = oldValue;
        ValueChange = valueChange;
        ActionType = actionType;
        ActionDate = actionDate;
    }

    public BalanceHistory(
        int accountId,
        decimal oldValue,
        decimal valueChange,
        ActionType actionType
    ) : this(
        accountId,
        oldValue,
        valueChange,
        actionType.ToString().ToLower(),
        DateTime.UtcNow
    ) { }
}
