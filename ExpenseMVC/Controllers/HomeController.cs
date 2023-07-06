using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ExpenseMVC.Models;
using ExpenseMVC.BusinessLogicServices.ExpenseServiceLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OutputCaching;

namespace ExpenseMVC.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IExpenseDataService _service;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ILogger<HomeController> logger, IExpenseDataService service, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _service = service;
        _userManager = userManager;
    }

    private async Task<string> GetUser()
    {
        var result = await _userManager.FindByEmailAsync(User?.Identity?.Name!);
        return result.Id;
    }

    [OutputCache(Duration = 60)]
    public async Task<IActionResult> Index()
    {
        var userId = await GetUser().ConfigureAwait(false);
        ViewBag.Total = _service.GetExpenseTotalForLastMonth(userId);
        ViewBag.MeanTotalOver55Days = await _service.GetMeanSpendByDays(userId, 55).ConfigureAwait(false);
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

