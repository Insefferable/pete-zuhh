using System.ComponentModel.DataAnnotations;

namespace PizzaStore.Api.DTOs;

public class UpdatePizzaDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Range(0, 10000)]
    public decimal Price { get; set; }
}