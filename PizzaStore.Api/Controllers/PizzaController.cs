using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Api.Data;
using PizzaStore.Api.DTOs;
using PizzaStore.Api.Models;

namespace PizzaStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PizzasController : ControllerBase
{
    private readonly AppDbContext _context;

    public PizzasController(AppDbContext context)
    {
        _context = context;
    }

    private static PizzaDto ToDto(Pizza pizza) => new()
    {
        Id = pizza.Id,
        Name = pizza.Name,
        Description = pizza.Description,
        Price = pizza.Price,
        Toppings = pizza.PizzaToppings
            .Select(pt => new ToppingDto { Id = pt.Topping.Id, Name = pt.Topping.Name })
            .ToList()
    };

    // GET: api/pizzas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PizzaDto>>> GetPizzas()
    {
        var pizzas = await _context.Pizzas
            .Include(p => p.PizzaToppings)
                .ThenInclude(pt => pt.Topping)
            .ToListAsync();

        return Ok(pizzas.Select(ToDto));
    }

    // GET: api/pizzas/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PizzaDto>> GetPizza(int id)
    {
        var pizza = await _context.Pizzas
            .Include(p => p.PizzaToppings)
                .ThenInclude(pt => pt.Topping)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pizza is null)
            return NotFound(new { message = $"Pizza with id {id} not found." });

        return Ok(ToDto(pizza));
    }

    // POST: api/pizzas
    [HttpPost]
    public async Task<ActionResult<PizzaDto>> CreatePizza(CreatePizzaDto dto)
    {
        var normalizedName = dto.Name.Trim();

        var duplicateExists = await _context.Pizzas
            .AnyAsync(p => p.Name.ToLower() == normalizedName.ToLower());

        if (duplicateExists)
            return Conflict(new { message = $"A pizza named '{normalizedName}' already exists." });

        var toppingIds = dto.ToppingIds.Distinct().ToList();
        var existingToppings = await _context.Toppings
            .Where(t => toppingIds.Contains(t.Id))
            .ToListAsync();

        if (existingToppings.Count != toppingIds.Count)
        {
            var missingIds = toppingIds.Except(existingToppings.Select(t => t.Id));
            return BadRequest(new { message = $"Invalid topping id(s): {string.Join(", ", missingIds)}" });
        }

        var pizza = new Pizza
        {
            Name = normalizedName,
            Description = dto.Description,
            Price = dto.Price,
            PizzaToppings = existingToppings
                .Select(t => new PizzaTopping { ToppingId = t.Id })
                .ToList()
        };

        _context.Pizzas.Add(pizza);
        await _context.SaveChangesAsync();

        // reload with toppings included for the response
        await _context.Entry(pizza)
            .Collection(p => p.PizzaToppings)
            .Query()
            .Include(pt => pt.Topping)
            .LoadAsync();

        return CreatedAtAction(nameof(GetPizza), new { id = pizza.Id }, ToDto(pizza));
    }

    // PUT: api/pizzas/5  (details only, not toppings)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePizza(int id, UpdatePizzaDto dto)
    {
        var pizza = await _context.Pizzas.FindAsync(id);
        if (pizza is null)
            return NotFound(new { message = $"Pizza with id {id} not found." });

        var normalizedName = dto.Name.Trim();

        var duplicateExists = await _context.Pizzas
            .AnyAsync(p => p.Id != id && p.Name.ToLower() == normalizedName.ToLower());

        if (duplicateExists)
            return Conflict(new { message = $"A pizza named '{normalizedName}' already exists." });

        pizza.Name = normalizedName;
        pizza.Description = dto.Description;
        pizza.Price = dto.Price;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // PUT: api/pizzas/5/toppings
    [HttpPut("{id:int}/toppings")]
    public async Task<IActionResult> UpdatePizzaToppings(int id, UpdatePizzaToppingsDto dto)
    {
        var pizza = await _context.Pizzas
            .Include(p => p.PizzaToppings)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pizza is null)
            return NotFound(new { message = $"Pizza with id {id} not found." });

        var toppingIds = dto.ToppingIds.Distinct().ToList();
        var existingToppings = await _context.Toppings
            .Where(t => toppingIds.Contains(t.Id))
            .ToListAsync();

        if (existingToppings.Count != toppingIds.Count)
        {
            var missingIds = toppingIds.Except(existingToppings.Select(t => t.Id));
            return BadRequest(new { message = $"Invalid topping id(s): {string.Join(", ", missingIds)}" });
        }

        pizza.PizzaToppings.Clear();
        foreach (var topping in existingToppings)
        {
            pizza.PizzaToppings.Add(new PizzaTopping { PizzaId = id, ToppingId = topping.Id });
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/pizzas/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePizza(int id)
    {
        var pizza = await _context.Pizzas.FindAsync(id);
        if (pizza is null)
            return NotFound(new { message = $"Pizza with id {id} not found." });

        _context.Pizzas.Remove(pizza);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}