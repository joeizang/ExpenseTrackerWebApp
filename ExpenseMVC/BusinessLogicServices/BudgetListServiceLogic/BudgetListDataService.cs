using ExpenseMVC.Data;
using ExpenseMVC.Models;
using ExpenseMVC.ViewModels.BudgetListVM;
using Microsoft.EntityFrameworkCore;

namespace ExpenseMVC.BusinessLogicServices.BudgetListServiceLogic;


public interface IBudgetListDataService
{
    Task<IEnumerable<BudgetListViewModel>> GetUserBudgetLists(string userId);

    Task AddNewBudgetList(BudgetList entity);
    Task<bool> UpdateBudgetList(BudgetList entity);
    Task DeleteBudgetList(BudgetList entity);
}

public class BudgetListDataService : IBudgetListDataService
{
    private readonly ApplicationDbContext _context;

    public BudgetListDataService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BudgetListViewModel>> GetUserBudgetLists(string userId)
    {
        var results = new List<BudgetListViewModel>();
        await foreach (var budgetList in CompiledLinqQueries.GetUserBudgetLists(_context, userId))
        {
            results.Add(budgetList);
        }

        return results;
    }

    public int CountBudgetListItems(string userId, Guid id)
    {
        var result = CompiledLinqQueries
            .GetUserBudgetListsForCount(_context, userId, id)
            .SingleOrDefault()?.BudgetItems.Count;
        return result.Value;
    }

    public async Task AddNewBudgetList(BudgetList entity)
    {
        _context.BudgetLists.Add(entity);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<bool> UpdateBudgetList(BudgetList entity)
    {
        try
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task DeleteBudgetList(BudgetList entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}