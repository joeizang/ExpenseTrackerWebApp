using ExpenseMVC.Data;
using ExpenseMVC.Models;
using ExpenseMVC.ViewModels.BudgetListVM;
using ExpenseMVC.ViewModels.ExpenseVM;
using Microsoft.EntityFrameworkCore;

namespace ExpenseMVC.BusinessLogicServices;

public class CompiledLinqQueries
{
    public static readonly Func<ApplicationDbContext, string, int>
        GetNnumberOfUserExpensesAsync =
            EF.CompileQuery(
                (ApplicationDbContext context, string userId) =>
                    context.Expenses
                        .AsNoTracking().Count(e => e.ExpenseOwner.Id == userId));


    public static readonly Func<ApplicationDbContext, string, IAsyncEnumerable<ExpenseIndexViewModel>>
        GetUserExpensesAsync =
            EF.CompileAsyncQuery(
                (ApplicationDbContext context, string userId) =>
                    context.Expenses.AsNoTracking()
                        .OrderBy(e => e.ExpenseDate)
                        .Where(e => e.ExpenseOwner.Id == userId)
                        .Select(e => new ExpenseIndexViewModel(e.Name, e.Description, e.Amount, e.ExpenseDate, e.Id,
                            e.CurrencyUsed, e.ExpenseType, e.Notes)));
    
    public static readonly Func<ApplicationDbContext, string, DateTimeOffset, IAsyncEnumerable<ExpenseIndexViewModel>>
        GetUserExpensesCursorPagedAsync =
            EF.CompileAsyncQuery(
                (ApplicationDbContext context, string userId, DateTimeOffset cursor) =>
                    context.Expenses.AsNoTracking()
                        .OrderBy(e => e.ExpenseDate)
                        .Where(e => e.ExpenseOwner.Id == userId)
                        .Where(e => e.ExpenseDate >= cursor)
                        .Select(e => new ExpenseIndexViewModel(e.Name, e.Description, e.Amount, e.ExpenseDate, e.Id,
                            e.CurrencyUsed, e.ExpenseType, e.Notes)));
    
    public static readonly Func<ApplicationDbContext, int, string, IAsyncEnumerable<ExpenseIndexViewModel>>
        GetUserExpensesFilteredByExpenseTypeAsync =
            EF.CompileAsyncQuery(
                (ApplicationDbContext context, int skipValue, string userId) =>
                    context.Expenses.AsNoTracking()
                        .OrderBy(e => e.ExpenseType)
                        .ThenBy(e => e.ExpenseDate)
                        .Where(e => e.ExpenseOwnerId == userId)
                        .Skip(skipValue)
                        .Select(e => new ExpenseIndexViewModel(e.Name, e.Description, e.Amount, e.ExpenseDate, e.Id,
                            e.CurrencyUsed, e.ExpenseType, e.Notes))
                        .Take(7));
    
    public static readonly Func<ApplicationDbContext, int, string, IAsyncEnumerable<ExpenseIndexViewModel>>
        GetUserExpensesFilteredByCurrencUsedAsync =
            EF.CompileAsyncQuery(
                (ApplicationDbContext context, int skipValue, string userId) =>
                    context.Expenses.AsNoTracking()
                        .OrderBy(e => e.CurrencyUsed)
                        .ThenBy(e => e.ExpenseDate)
                        .Where(e => e.ExpenseOwnerId == userId)
                        .Skip(skipValue)
                        .Select(e => new ExpenseIndexViewModel(e.Name, e.Description, e.Amount, e.ExpenseDate, e.Id,
                            e.CurrencyUsed, e.ExpenseType, e.Notes))
                        .Take(7));
    
    public static readonly Func<ApplicationDbContext, string, IEnumerable<ExpenseDashBoardSummary>>
        GetExpenseTotalForLast30DaysAsync =
            EF.CompileQuery(
                (ApplicationDbContext context, string userId) =>
                    context.Expenses.AsNoTracking()
                        .Where(e => e.ExpenseOwnerId == userId)
                        .Where(e => e.ExpenseDate >= DateTime.Now.AddDays(-30))
                        .Select(e => new ExpenseDashBoardSummary(e.Amount, e.CurrencyUsed)));
    
    public static readonly Func<ApplicationDbContext, string, int, IAsyncEnumerable<ExpenseDashBoardSummary>>
        GetMeanSpendOverSpecifiedPeriod =
            EF.CompileAsyncQuery(
                (ApplicationDbContext context, string userId, int days) =>
                    context.Expenses.AsNoTracking()
                        .Where(e => e.ExpenseOwnerId == userId)
                        .Where(e => e.ExpenseDate >= DateTime.Now.AddDays(-days))
                        .GroupBy(e => e.CurrencyUsed)
                        .Select(e => new ExpenseDashBoardSummary(e.Average(e => e.Amount), e.Key))
            );
// BUDGETLIST QUERIES //
    public static readonly Func<ApplicationDbContext, string, IAsyncEnumerable<BudgetListViewModel>>
        GetUserBudgetLists =
            EF.CompileAsyncQuery(
                (ApplicationDbContext context, string userId) =>
                    context.BudgetLists.AsNoTracking()
                        .OrderBy(b => b.CreatedAt)
                        .Where(b => b.Expense.ExpenseOwnerId == userId)
                        .Select(b => new BudgetListViewModel(b.Id, b.ListName, b.Description,
                            b.Expense.ExpenseOwnerId, b.BudgetItems.Select(x => new BudgetListItemViewModel(
                                x.Id, x.Description, x.Quantity, x.Price, x.UnitPrice, x.Description))))
            );
    
    public static readonly Func<ApplicationDbContext, string, Guid, IEnumerable<BudgetList>>
        GetUserBudgetListsForCount =
            EF.CompileQuery(
                (ApplicationDbContext context, string userId, Guid id) =>
                    context.BudgetLists
                        .AsNoTracking()
                        .Where(b => b.Expense.ExpenseOwnerId == userId)
                        .Where(b => b.Id == id)
            );

    // public static readonly Func<ApplicationDbContext, string, Guid, int>
    //     GetUserBudgetListItems =
    //         EF.CompileQuery(
    //             (ApplicationDbContext context, string userId, Guid id) =>
    //                 context.BudgetLists.AsNoTracking()
    //                     .Include(x => x.BudgetItems)
    //                     .Where(x => x.Expense.ExpenseOwnerId == userId)
    //                     .Count(x => x.BudgetItems.Where(i => i.))
    //         );
}
