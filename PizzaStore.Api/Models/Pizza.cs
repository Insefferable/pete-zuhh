using System.ComponentModel.DataAnnotations;

namespace PizzaStore.Api.Models;

public class Pizza
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public decimal Price { get; set; }

    public ICollection<PizzaTopping> PizzaToppings { get; set; } = new List<PizzaTopping>();
}