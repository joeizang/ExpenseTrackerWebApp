using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseMVC.Data;
using ExpenseMVC.Models;
using ExpenseMVC.ViewModels.ExpenseVM;
using FluentValidation;
using System.Diagnostics;
using ExpenseMVC.BusinessLogicServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ExpenseMVC.BusinessLogicServices.ExpenseServiceLogic;
using Microsoft.AspNetCore.OutputCaching;

namespace ExpenseMVC.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IValidator<CreateExpenseViewModel> _createValidator;
        // private readonly UserManager<ApplicationUser> _userManager;
        private readonly IExpenseDataService _expenseDataService;
        private readonly IUserService _userService;

        public ExpenseController(ApplicationDbContext context,
            IValidator<CreateExpenseViewModel> createValidator,
            // UserManager<ApplicationUser> userManager,
            IExpenseDataService expenseDataService,
            IUserService userService)
        {
            _context = context;
            _createValidator = createValidator;
            // _userManager = userManager;
            _expenseDataService = expenseDataService;
            _userService = userService;
        }

        private ExpenseController() { }

        // GET: Expense
        [OutputCache(Duration = 60)]
        public async Task<IActionResult> Index(PagedResult<ExpenseIndexViewModel> model)
        {
            PagedResult<ExpenseIndexViewModel> expenses = default!;
            var skipValue = (model.PageNumber - 1) * model.PageSize;
            var currentlyLoggedInUser = await _userService.GetLoggedInUser().ConfigureAwait(false);
            if (model.CurrentFilter == FilterCriteria.ByDate)
                expenses = await _expenseDataService.GetUserExpenses(currentlyLoggedInUser.Id).ConfigureAwait(false);
            if (model.CurrentFilter == FilterCriteria.ByExpenseType)
                expenses = await _expenseDataService.GetExpenseTypeFilteredExpenses(currentlyLoggedInUser.Id, skipValue)
                            .ConfigureAwait(false);
            if (model.CurrentFilter == FilterCriteria.ByCurrencyUsed)
                expenses = await _expenseDataService.GetCurrencyUsedFilteredExpenses(currentlyLoggedInUser.Id, skipValue)
                            .ConfigureAwait(false);
            return View(expenses);
        }

        // GET: Expense/Create
        public IActionResult Create()
        {
            var model = new CreateExpenseViewModel();
            model.ExpenseDate = DateTimeOffset.UtcNow.ToLocalTime();
            return View(model);
        }

        // POST: Expense/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateExpenseViewModel expense)
        {
            var validationResult = await _createValidator.ValidateAsync(expense);
            var currentlyLoggedInUser = await _userService.GetLoggedInUser().ConfigureAwait(false);

            if (validationResult.IsValid)
            {
                Debug.WriteLine("Validation passed");
                var expenseEntity = new Expense
                {
                    ExpenseDate = expense.ExpenseDate,
                    Name = expense.ExpenseName,
                    Description = expense.ExpenseDescription,
                    Amount = expense.ExpenseAmount,
                    CurrencyUsed = expense.ExpenseCurrencyUsed,
                    Notes = expense.ExpenseNotes,
                    ExpenseType = expense.ExpenseType,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                    ExpenseOwnerId = currentlyLoggedInUser.Id
                };

                _context.Add(expenseEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        // GET: Expense/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var expense = await _context.Expenses.Include(x => x.ExpenseOwner)
                            .SingleOrDefaultAsync(x => x.Id == id);
            if (expense == null)
            {
                return NotFound();
            }

            var mappedExpense = new ExpenseUpdateViewModel(
                expense.Name,
                expense.Description,
                expense.Amount,
                expense.ExpenseDate,
                expense.Id,
                expense.CurrencyUsed,
                expense.ExpenseType,
                expense.Notes ?? string.Empty,
                expense!.ExpenseOwner!.Email!
            );

            return View(mappedExpense!);
        }

        // POST: Expense/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ExpenseUpdateViewModel expense)
        {
            var user = await _userService.UserManager.FindByEmailAsync(expense.ExpenseOwnerEmail);
            if (id != expense.ExpenseId)
            {
                return NotFound();
            }
            var transaction = await _context.Database.BeginTransactionAsync();
            if (!ModelState.IsValid) return View(expense);
            try
            {
                var expenseEntity = await _context.Expenses.FindAsync(id);
                if (expenseEntity is null)
                {
                    return NotFound("Could not find the expense you are looking for!"); //add logging.
                }
                expenseEntity.Name = expense.ExpenseName;
                expenseEntity.Description = expense.ExpenseDescription;
                expenseEntity.Amount = expense.Amount;
                expenseEntity.CurrencyUsed = expense.CurrencyUsed;
                expenseEntity.Notes = expense.Notes ?? string.Empty;
                expenseEntity.ExpenseType = expense.ExpenseType;
                expenseEntity.UpdatedAt = DateTimeOffset.UtcNow;
                _context.Entry(expenseEntity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseExists(expense.ExpenseId))
                {
                    return NotFound();
                }
                else
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Expense/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Expenses == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses
                .Include(e => e.ExpenseOwner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expense == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", expense);
        }

        // POST: Expense/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Expenses == null)
            {
                TempData["Message"] = "Your expenses are empty so you shouldn't have anything to delete.";
                return Problem("Your expenses are empty so you shouldn't have anything to delete.");
            }
            var expense = await _context.Expenses.FindAsync(id);
            if (expense is not null)
            {
                _context.Expenses.Remove(expense);
            }
            TempData["Message"] = "Expense deleted successfully";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpenseExists(Guid id)
        {
            return (_context.Expenses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
