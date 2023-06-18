namespace ExpenseMVC.Models;

public class Income : BaseEntity
{
    public decimal Amount { get; set; }
    
    public Currency CurrencyUsed { get; set; }
    
    public string? Notes { get; set; } = string.Empty;
    
    public ApplicationUser IncomeOwner { get; set; }
    
    public string IncomeOwnerId { get; set; } = string.Empty;
    
    
    
}