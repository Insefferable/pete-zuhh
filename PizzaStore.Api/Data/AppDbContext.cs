using Microsoft.EntityFrameworkCore;
using PizzaStore.Api.Models;

namespace PizzaStore.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Pizza> Pizzas => Set<Pizza>();
    public DbSet<Topping> Toppings => Set<Topping>();
    public DbSet<PizzaTopping> PizzaToppings => Set<PizzaTopping>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PizzaTopping>()
            .HasKey(pt => new { pt.PizzaId, pt.ToppingId });

        modelBuilder.Entity<PizzaTopping>()
            .HasOne(pt => pt.Pizza)
            .WithMany(p => p.PizzaToppings)
            .HasForeignKey(pt => pt.PizzaId);

        modelBuilder.Entity<PizzaTopping>()
            .HasOne(pt => pt.Topping)
            .WithMany(t => t.PizzaToppings)
            .HasForeignKey(pt => pt.ToppingId);

        // Enforce "prevent duplicate names" at the database level too
        modelBuilder.Entity<Pizza>()
            .HasIndex(p => p.Name)
            .IsUnique();

        modelBuilder.Entity<Topping>()
            .HasIndex(t => t.Name)
            .IsUnique();
    }
}