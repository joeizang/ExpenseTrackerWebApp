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

    public int TotalExpenses { get; set; }

    public int TotalCount(string userId)
    {
        return CompiledLinqQueries.GetNnumberOfUserExpensesAsync(_context, userId);
    }

    public async Task<PagedResult<ExpenseIndexViewModel>> GetUserExpenses(string userId, int pageNumber = 1)
    {
        var expenses = new List<ExpenseIndexViewModel>();
        TotalExpenses = TotalCount(userId);
        await foreach(var each in CompiledLinqQueries.GetUserExpensesAsync(_context, userId))
        {
            expenses.Add(each);
        }

        return new PagedResult<ExpenseIndexViewModel>(expenses, TotalExpenses, pageNumber, pageNumber, 
                (int)Math.Ceiling(TotalExpenses / (double)7), 7, pageNumber < (int)Math.Ceiling(TotalExpenses / (double)7), pageNumber > 1);
    }

    public async Task<CursorPagedResult<List<ExpenseIndexViewModel>>> GetUserExpensesCursorPaged(string userId,
        DateTimeOffset cursor)
    {
        var expenses = new List<ExpenseIndexViewModel>();
        await foreach (var each in CompiledLinqQueries.GetUserExpensesCursorPagedAsync(_context, userId, cursor))
        {
            expenses.Add(each);
        }

        return new CursorPagedResult<List<ExpenseIndexViewModel>>(expenses[^1].ExpenseDate, expenses);
    }

    public async Task<PagedResult<ExpenseIndexViewModel>> GetExpenseTypeFilteredExpenses(string userId, int pageNumber)
    {
        var expenses = new List<ExpenseIndexViewModel>();
        TotalExpenses = TotalCount(userId);
        await foreach(var each in CompiledLinqQueries.GetUserExpensesFilteredByExpenseTypeAsync(_context, pageNumber, userId))
        {
            expenses.Add(each);
        }
        return new PagedResult<ExpenseIndexViewModel>(expenses, TotalExpenses, pageNumber, pageNumber, 
                (int)Math.Ceiling(TotalExpenses / (double)7), 7, pageNumber < (int)Math.Ceiling(TotalExpenses / (double)7), pageNumber > 1);
    }

    public async Task<PagedResult<ExpenseIndexViewModel>> GetCurrencyUsedFilteredExpenses(string userId, int pageNumber)
    {
        var expenses = new List<ExpenseIndexViewModel>();
        TotalExpenses = TotalCount(userId);
        await foreach(var each in CompiledLinqQueries.GetUserExpensesFilteredByCurrencUsedAsync(_context, pageNumber, userId))
        {
            expenses.Add(each);
        }
        return new PagedResult<ExpenseIndexViewModel>(expenses, TotalExpenses, pageNumber, pageNumber, 
                (int)Math.Ceiling(TotalExpenses / (double)7), 7, pageNumber < (int)Math.Ceiling(TotalExpenses / (double)7), pageNumber > 1);
    }

    public decimal GetExpenseTotalForLastMonth(string userId)
    {
        var result = CompiledLinqQueries.GetExpenseTotalForLast30DaysAsync(_context, userId);
        var actualTotal = 0m;
        foreach(var each in result)
        {
            if(each.CurrencyUsed == Currency.USD)
            {
                actualTotal += each.Amount * 750;
            } else if(each.CurrencyUsed == Currency.EUR)
            {
                actualTotal += each.Amount * 900;
            } else if(each.CurrencyUsed == Currency.GBP)
            {
                actualTotal += each.Amount * 1050;
            } else if(each.CurrencyUsed == Currency.NGN)
            {
                actualTotal += each.Amount;
            }
        }
        return actualTotal;
    }

    public async Task<decimal> GetMeanSpendByDays(string userId, int days)
    {
        var result = 0m;
        var counter = 0;
        await foreach (var item in CompiledLinqQueries.GetMeanSpendOverSpecifiedPeriod(_context, userId, days))
        {
            var temp = item.CurrencyUsed switch
            {
                Currency.USD => item.Amount * 750,
                Currency.EUR => item.Amount * 900,
                Currency.GBP => item.Amount * 1050,
                _ => item.Amount
            };
            result += temp;
            counter++;
        }
        return result;
    }
}

public interface IExpenseDataService
{
    decimal GetExpenseTotalForLastMonth(string userId);
    Task<PagedResult<ExpenseIndexViewModel>> GetUserExpenses(string userId, int pageNumber = 1);

    Task<PagedResult<ExpenseIndexViewModel>> GetCurrencyUsedFilteredExpenses(string userId, int pageNumber);

    Task<PagedResult<ExpenseIndexViewModel>> GetExpenseTypeFilteredExpenses(string userId, int pageNumber);

    Task<decimal> GetMeanSpendByDays(string userId, int days);
}