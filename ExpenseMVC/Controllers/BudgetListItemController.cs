using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseMVC.Data;
using ExpenseMVC.Models;

namespace ExpenseMVC.Controllers
{
    public class BudgetListItemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BudgetListItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BudgetListItem
        public async Task<IActionResult> Index()
        {
              return _context.BudgetLists != null ? 
                          View(await _context.BudgetLists.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.BudgetLists'  is null.");
        }

        // GET: BudgetListItem/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.BudgetLists == null)
            {
                return NotFound();
            }

            var budgetList = await _context.BudgetLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (budgetList == null)
            {
                return NotFound();
            }

            return View(budgetList);
        }

        // GET: BudgetListItem/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BudgetListItem/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ListName,Description,ExpenseEntityId,Id,CreatedAt,UpdatedAt")] BudgetList budgetList)
        {
            if (ModelState.IsValid)
            {
                budgetList.Id = Guid.NewGuid();
                _context.Add(budgetList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(budgetList);
        }

        // GET: BudgetListItem/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.BudgetLists == null)
            {
                return NotFound();
            }

            var budgetList = await _context.BudgetLists.FindAsync(id);
            if (budgetList == null)
            {
                return NotFound();
            }
            return View(budgetList);
        }

        // POST: BudgetListItem/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ListName,Description,ExpenseEntityId,Id,CreatedAt,UpdatedAt")] BudgetList budgetList)
        {
            if (id != budgetList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(budgetList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BudgetListExists(budgetList.Id))
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
            return View(budgetList);
        }

        // GET: BudgetListItem/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.BudgetLists == null)
            {
                return NotFound();
            }

            var budgetList = await _context.BudgetLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (budgetList == null)
            {
                return NotFound();
            }

            return View(budgetList);
        }

        // POST: BudgetListItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.BudgetLists == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BudgetLists'  is null.");
            }
            var budgetList = await _context.BudgetLists.FindAsync(id);
            if (budgetList != null)
            {
                _context.BudgetLists.Remove(budgetList);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BudgetListExists(Guid id)
        {
          return (_context.BudgetLists?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
