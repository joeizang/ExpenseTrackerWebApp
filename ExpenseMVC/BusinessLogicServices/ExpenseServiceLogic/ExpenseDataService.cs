using ExpenseMVC.Data;
using ExpenseMVC.Models;
using ExpenseMVC.ViewModels.ExpenseVM;
using Microsoft.EntityFrameworkCore;

namespace ExpenseMVC.BusinessLogicServices.ExpenseServiceLogic;

public class ExpenseDataService : IExpenseDataService
{
    private readonly ApplicationDbContext _context;

    public ExpenseDataService(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<ExpenseIndexViewModel> GetUserExpenses(string userId, CancellationToken token)
    {
        var expenses = new List<ExpenseIndexViewModel>();
        foreach(var each in CompiledLinqQueries.GetUserExpensesAsync(_context, userId, token))
        {
            expenses.Add(each);
        }
        return expenses;
    }
}

public interface IExpenseDataService
{
    List<ExpenseIndexViewModel> GetUserExpenses(string userId, CancellationToken token);
}