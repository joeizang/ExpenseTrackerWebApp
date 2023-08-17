namespace ExpenseMVC.ViewModels.BudgetListVM;

public record BudgetListItemViewModel(Guid ItemId, string Name, double Quantity, decimal Price, decimal UnitPrice, string Description);
public record CreateBudgetListItemViewModel(string Name, double Quantity, decimal Price,
    decimal UnitPrice, string Description);