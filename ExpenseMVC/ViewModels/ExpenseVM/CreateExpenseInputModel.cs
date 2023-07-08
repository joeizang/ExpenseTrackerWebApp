using System.ComponentModel.DataAnnotations;
using ExpenseMVC.Models;


namespace ExpenseMVC.ViewModels.ExpenseVM;

public class CreateExpenseViewModel
{
    [Required]
    [Display(Name = "Expense Name")]
    public string ExpenseName { get; set; } = default!;

    [Required]
    [Display(Name = "Expense Amount")]
    public decimal ExpenseAmount { get; set; }

    [Required]
    [Display(Name = "Expense Description")]
    public string ExpenseDescription { get; set; } = default!;

    [Required]
    [Display(Name = "Expense Currency")]
    public Currency ExpenseCurrencyUsed { get; set; } = default!;

    [Required]
    [Display(Name = "Notes on Expense")]
    public string ExpenseNotes { get; set; } = default!;

    [Required]
    [Display(Name = "Expense Type")]
    public ExpenseType ExpenseType { get; set; }

    [Required]
    [Display(Name = "Expense Date")]
    [DataType(DataType.Date)]
    public DateTimeOffset ExpenseDate { get; set; }
}

public class CreateExpenseInputModel
{
    [Required]
    [Display(Name = "Expense Name")]
    public string ExpenseName { get; set; } = default!;

    [Required]
    [Display(Name = "Expense Amount")]
    public decimal ExpenseAmount { get; set; }

    [Required]
    [Display(Name = "Expense Description")]
    public string ExpenseDescription { get; set; } = default!;

    [Required]
    [Display(Name = "Expense Type")]
    public ExpenseType ExpenseType { get; set; }

    [Required]
    [Display(Name = "Expense Type Selected")]
    public int ExpenseTypeSelected { get; set; }

    public Currency CurrencyUsed { get; set; }



    [Required]
    [Display(Name = "Expense Date")]
    public DateTime ExpenseDate { get; set; }

    [Required]
    [Display(Name = "Expense Owner")]
    public string ExpenseOwnerId { get; set; } = default!;
}

public record ExpenseIndexViewModel(string ExpenseName, string ExpenseDescription, decimal ExpenseAmount, DateTimeOffset ExpenseDate, Guid Id, Currency CurrencyUsed, ExpenseType ExpenseType, string ExpenseNotes);

public record ExpenseUpdateViewModel(string ExpenseName, string ExpenseDescription, decimal Amount, DateTimeOffset ExpenseDate,
    Guid ExpenseId, Currency CurrencyUsed, ExpenseType ExpenseType, string Notes, string ExpenseOwnerEmail);

public record ExpenseDashBoardSummary(decimal Amount, Currency CurrencyUsed);
