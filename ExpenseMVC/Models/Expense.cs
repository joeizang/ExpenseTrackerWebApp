namespace ExpenseMVC.Models;

public class Expense : BaseEntity
{
    public DateTimeOffset ExpenseDate { get; set; }

    public string Description { get; set; } = string.Empty;
    
    public decimal Amount { get; set; }

    public Currency CurrencyUsed { get; set; }

    public string? ReceiptUrl { get; set; } = string.Empty;
    
    public string? Notes { get; set; } = string.Empty;

    public ExpenseType ExpenseType { get; set; }

    public ApplicationUser ExpenseOwner { get; set; }
    
    public string ExpenseOwnerId { get; set; } = string.Empty;
    
    public Expense()
    {
        ExpenseDate = DateTimeOffset.Now;
    }

}