using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyTrade.BrokerService.Entities.Products;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public decimal Ppt { get; set; }
    public string Currency { get; set; } = string.Empty;

    public Product() { }

    public Product(string name, decimal ppt, string currency)
    {
        Name = name;
        Ppt = ppt;
        Currency = currency;
    }

    public Product(int id, string name, decimal ppt, string currency)
        : this(name, ppt, currency)
    {
        Id = id;
    }
}
