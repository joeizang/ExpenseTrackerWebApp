using ExpenseMVC.Data;
using ExpenseMVC.Models;
using ExpenseMVC.ViewModels.ExpenseVM;
using Microsoft.EntityFrameworkCore;

namespace ExpenseMVC.BusinessLogicServices;

public class CompiledLinqQueries
{
    public static Func<ApplicationDbContext, string, CancellationToken, IEnumerable<ExpenseIndexViewModel>>
        GetUserExpensesAsync =
            EF.CompileQuery(
                (ApplicationDbContext context, string userId, CancellationToken token) =>
                    context.Expenses.AsNoTracking()
                        .Where(e => e.ExpenseOwner.Id == userId)
                        .OrderBy(e => e.ExpenseDate)
                        .Select(e => new ExpenseIndexViewModel(e.Name, e.Description, e.Amount, e.ExpenseDate, e.Id,
                            e.CurrencyUsed, e.ExpenseType, e.Notes)));

    // public static IQueryable<ExpenseIndexViewModel> RunQuery(ApplicationDbContext context, string userId, CancellationToken token)
    // {
    //     var result = context.Expenses.AsNoTracking()
    //         .Where(e => e.ExpenseOwner.Id == userId)
    //         .OrderBy(e => e.ExpenseDate)
    //         .Select(e => 
    //             new ExpenseIndexViewModel(e.Name, e.Description, e.Amount, e.CurrencyUsed.ToString(),
    //             e.ExpenseType.ToString(), e.ExpenseDate.ToString(), e.Id));
    //     return result;
    // }

    // public static Func<ApplicationDbContext, Guid, string, CancellationToken, IEnumerable<ExpenseUpdateViewModel?>> GetExpenseById = 
    //     EF.CompileAsyncQuery((ApplicationDbContext context, Guid id, string ownerId, CancellationToken token) =>
    //         context.Expenses.AsNoTracking()
    //             .Include(e => e.ExpenseOwner)
    //             .Where(e => e.Id == id)
    //             .Select(e => new ExpenseUpdateViewModel(e.Name, e.Description, e.Notes ?? "", 
    //                 e.Amount, e.CurrencyUsed.ToString(), e.ExpenseType.ToString(), 
    //                 e.ExpenseDate.ToString(), "", ownerId, e.Id))
    //     );

    // public static Func<ApplicationDbContext, Task<List<Expense>>> GetAllExpenses = 
    //     EF.CompileAsyncQuery((ApplicationDbContext context) =>
    //         context.Expenses
    //             .Include(e => e.ExpenseOwner)
    //             .ToListAsync()
    //     );
}
