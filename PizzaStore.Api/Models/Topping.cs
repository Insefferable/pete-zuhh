using System.ComponentModel.DataAnnotations;

namespace PizzaStore.Api.Models;

public class Topping
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<PizzaTopping> PizzaToppings { get; set; } = new List<PizzaTopping>();
}