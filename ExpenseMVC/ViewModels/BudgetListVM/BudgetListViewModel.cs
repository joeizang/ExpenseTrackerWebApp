namespace ExpenseMVC.ViewModels.BudgetListVM;

public record BudgetListViewModel(Guid ListId, string Name, string Description, string OwnerId, 
    IEnumerable<BudgetListItemViewModel> Items);
    
public record BudgetListIndexViewModel(Guid ListId, string Name, string Description, string OwnerId, int ItemCount, 
    IEnumerable<BudgetListItemViewModel> Items);

public class CreateBudgetListViewModel
{
    public required string Name { get; set; }

    public string Description { get; set; } = string.Empty;

    public required List<BudgetListItemViewModel> Items { get; set; }
}