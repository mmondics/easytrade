using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyTrade.BrokerService.Entities.Packages;

public class Package
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Support { get; set; } = string.Empty;

    public Package() { }

    public Package(string name, decimal price, string support)
    {
        Name = name;
        Price = price;
        Support = support;
    }
}
