using ExpenseMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseMVC.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Expense> Expenses { get; set; } = null!;
    
    public DbSet<Income> Incomes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Expense>()
            .Property(e => e.Amount)
            .HasPrecision(2);
        builder.Entity<Income>()
            .Property(i => i.Amount)
            .HasPrecision(2);
        base.OnModelCreating(builder);
    }
}

