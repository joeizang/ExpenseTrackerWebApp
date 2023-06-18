using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseMVC.Data;
using ExpenseMVC.Models;
using ExpenseMVC.ViewModels.ExpenseVM;

namespace ExpenseMVC.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Expense
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Expenses.Include(e => e.ExpenseOwner);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Expense/Details/5
        public async Task<IActionResult> Details(Guid? id)
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

            return View(expense);
        }

        // GET: Expense/Create
        public IActionResult Create()
        {
            var model = new CreateExpenseViewModel();
            model.ExpenseDate = DateTimeOffset.UtcNow.ToLocalTime();
            model.ExpenseOwnerId = User?.Identity?.Name;
            return View(model);
        }

        // POST: Expense/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateExpenseInputModel expense)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExpenseOwnerId"] = new SelectList(_context.Users, "Id", "Id", expense.ExpenseOwnerId);
            return View(expense);
        }

        // GET: Expense/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Expenses == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }
            ViewData["ExpenseOwnerId"] = new SelectList(_context.Users, "Id", "Id", expense.ExpenseOwnerId);
            return View(expense);
        }

        // POST: Expense/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ExpenseDate,Description,Amount,CurrencyUsed,ReceiptUrl,Notes,ExpenseType,ExpenseOwnerId,Id,CreatedAt,UpdatedAt")] Expense expense)
        {
            if (id != expense.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(expense.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExpenseOwnerId"] = new SelectList(_context.Users, "Id", "Id", expense.ExpenseOwnerId);
            return View(expense);
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

            return View(expense);
        }

        // POST: Expense/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Expenses == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Expenses'  is null.");
            }
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpenseExists(Guid id)
        {
          return (_context.Expenses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
