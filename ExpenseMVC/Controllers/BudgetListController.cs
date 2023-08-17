using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseMVC.BusinessLogicServices;
using ExpenseMVC.BusinessLogicServices.BudgetListServiceLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseMVC.Data;
using ExpenseMVC.Models;
using ExpenseMVC.ViewModels.BudgetListVM;
using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ExpenseMVC.Controllers
{
    [Authorize]
    public class BudgetListController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBudgetListDataService _dataService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly ILogger<BudgetListController> _logger;

        public BudgetListController(ApplicationDbContext context, IBudgetListDataService dataService,
            UserManager<ApplicationUser> userManager, IUserService userService,
            ILogger<BudgetListController> logger)
        {
            _context = context;
            _dataService = dataService;
            _userManager = userManager;
            _userService = userService;
            _logger = logger;
        }

        // GET: BudgetList
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userService.GetLoggedInUser().ConfigureAwait(false);
            var results = await _dataService
                .GetUserBudgetLists(currentUser.Id)
                .ConfigureAwait(false);
            return View(results);
        }

        // GET: BudgetList/Details/5
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

        public IActionResult ListDetails(int id)
        {
            if(Request.IsHtmx()) _logger.LogInformation("Htmx is now banging!!!");
            return Request.IsHtmx() ? PartialView("_BudgetListDetails") : NotFound();
        }

        // GET: BudgetList/Create
        public IActionResult Create()
        {
            if(Request.IsHtmx()) _logger.LogInformation("HtMX activated!!");
            return View();
        }

        public IActionResult CreateBudgetListItem()
        {
            var model = new CreateBudgetListItemViewModel(string.Empty, 0d, 0m,
                0m, string.Empty);
            return PartialView("_CreateBudgetListItem", model);
        }

        // POST: BudgetList/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBudgetListViewModel budgetList)
        {
            // if(Request.IsHtmx()) _logger.LogInformation("htmx is active now!!");
            if (ModelState.IsValid)
            {
                // budgetList.Id = Guid.NewGuid();
                // _context.Add(budgetList);
                // await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(budgetList);
        }

        // GET: BudgetList/Edit/5
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

        // POST: BudgetList/Edit/5
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

        // GET: BudgetList/Delete/5
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

        // POST: BudgetList/Delete/5
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
