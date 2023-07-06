using ExpenseMVC.Data;
using ExpenseMVC.Models;
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
    
}
