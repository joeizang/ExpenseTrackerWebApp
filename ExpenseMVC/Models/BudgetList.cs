namespace ExpenseMVC.Models;

public class BudgetList : BaseEntity
{
    public required string ListName { get; set; }

    public string Description { get; set; } = string.Empty;

    public required List<BudgetListItem> BudgetItems { get; set; }

    public Guid ExpenseEntityId { get; set; }

    public Expense Expense { get; set; } = default!;
}