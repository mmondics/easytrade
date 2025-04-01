using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EasyTrade.BrokerService.ExceptionHandling.Exceptions;

namespace EasyTrade.BrokerService.Entities.Balances;

[Table("Balance")]
public class Balance
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int AccountId { get; set; }

    public decimal Value { get; set; }

    public Balance() { }

    public Balance(int accountId, decimal value)
    {
        AccountId = accountId;
        Value = value;
    }

    public void Deposit(decimal amount)
    {
        if (amount < 0)
        {
            throw new NegativeAmountException();
        }
        Value += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount < 0)
        {
            throw new NegativeAmountException();
        }
        else if (amount > Value)
        {
            throw new NotEnoughMoneyException(
                $"Not enough money to withdraw (missing {amount - Value})"
            );
        }
        Value -= amount;
    }
}
