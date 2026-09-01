namespace PizzaStore.Api.DTOs;

public class UpdatePizzaToppingsDto
{
    public List<int> ToppingIds { get; set; } = new();
}