using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Api.Data;
using PizzaStore.Api.DTOs;
using PizzaStore.Api.Infrastructure;
using PizzaStore.Api.Models;

namespace PizzaStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToppingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ToppingsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/toppings
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ToppingDto>>> GetToppings()
    {
        var toppings = await _context.Toppings
            .Select(t => new ToppingDto { Id = t.Id, Name = t.Name })
            .ToListAsync();

        return Ok(toppings);
    }

    // GET: api/toppings/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ToppingDto>> GetTopping(int id)
    {
        var topping = await _context.Toppings.FindAsync(id);

        if (topping is null)
            return NotFound(new { message = $"Topping with id {id} not found." });

        return Ok(new ToppingDto { Id = topping.Id, Name = topping.Name });
    }

    // POST: api/toppings
    [HttpPost]
    public async Task<ActionResult<ToppingDto>> CreateTopping(CreateToppingDto dto)
    {
        var normalizedName = dto.Name.Trim();

        var duplicateExists = await _context.Toppings
            .AnyAsync(t => t.Name.ToLower() == normalizedName.ToLower());

        if (duplicateExists)
            return Conflict(new { message = $"A topping named '{normalizedName}' already exists." });

        var topping = new Topping { Name = normalizedName };
        _context.Toppings.Add(topping);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            // Lost a race: another request inserted the same name between our
            // AnyAsync check and this SaveChangesAsync. The unique index caught it.
            return Conflict(new { message = $"A topping named '{normalizedName}' already exists." });
        }

        var result = new ToppingDto { Id = topping.Id, Name = topping.Name };
        return CreatedAtAction(nameof(GetTopping), new { id = topping.Id }, result);
    }

    // PUT: api/toppings/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTopping(int id, UpdateToppingDto dto)
    {
        var topping = await _context.Toppings.FindAsync(id);
        if (topping is null)
            return NotFound(new { message = $"Topping with id {id} not found." });

        var normalizedName = dto.Name.Trim();

        var duplicateExists = await _context.Toppings
            .AnyAsync(t => t.Id != id && t.Name.ToLower() == normalizedName.ToLower());

        if (duplicateExists)
            return Conflict(new { message = $"A topping named '{normalizedName}' already exists." });

        topping.Name = normalizedName;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            return Conflict(new { message = $"A topping named '{normalizedName}' already exists." });
        }

        return NoContent();
    }

    // DELETE: api/toppings/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTopping(int id)
    {
        var topping = await _context.Toppings.FindAsync(id);
        if (topping is null)
            return NotFound(new { message = $"Topping with id {id} not found." });

        _context.Toppings.Remove(topping);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}