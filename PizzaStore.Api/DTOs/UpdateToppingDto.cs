using System.ComponentModel.DataAnnotations;

namespace PizzaStore.Api.DTOs;

public class UpdateToppingDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}