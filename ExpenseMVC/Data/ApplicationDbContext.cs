﻿using ExpenseMVC.Models;
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

    public DbSet<BudgetList> BudgetLists { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Expense>()
            .Property(e => e.Amount)
            .HasPrecision(18,2);
        builder.Entity<Income>()
            .Property(i => i.Amount)
            .HasPrecision(18,2);
        builder.Entity<BudgetListItem>()
            .Property(b => b.UnitPrice)
            .HasPrecision(18, 2);
        builder.Entity<BudgetListItem>()
            .Property(b => b.Price)
            .HasPrecision(18, 2);

        builder.Entity<Expense>()
            .HasMany(e => e.ExpenseBudget)
            .WithOne()
            .HasForeignKey(x => x.ExpenseEntityId)
            .IsRequired(false);
        base.OnModelCreating(builder);
    }
}

