namespace ExpenseMVC.Models;

public class BudgetListItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; private set; }

    public decimal UnitPrice { get; set; }

    public double Quantity { get; set; }

    public string Description { get; set; } = string.Empty;

    public void GetItemPrice()
    {
        Price = UnitPrice * (decimal)Quantity;
    }
}