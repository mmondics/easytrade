using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyTrade.BrokerService.Entities.Instruments;

public class Instrument
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int ProductId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Instrument() { }

    public Instrument(string code, string name, string description)
    {
        Code = code;
        Name = name;
        Description = description;
    }

    public Instrument(int productId, string code, string name, string description)
        : this(code, name, description)
    {
        ProductId = productId;
    }

    public Instrument(int id, int productId, string code, string name, string description)
        : this(productId, code, name, description)
    {
        Id = id;
    }
}
