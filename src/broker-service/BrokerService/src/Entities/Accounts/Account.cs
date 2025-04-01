using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyTrade.BrokerService.Entities.Accounts;

public class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int PackageId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
    public DateTime PackageActivationDate { get; set; }
    public bool AccountActive { get; set; }
    public string Address { get; set; } = string.Empty;

    // Optional: parameterless constructor for EF Core
    public Account() { }
}
