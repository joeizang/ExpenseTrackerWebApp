using System.Diagnostics;
using ExpenseMVC.BusinessLogicServices;
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
    private readonly ThemeService _themeService;
    private readonly IConfiguration _config;

    public HomeController(
        ILogger<HomeController> logger,
        IExpenseDataService service,
        UserManager<ApplicationUser> userManager,
        ThemeService themeService,
        IConfiguration config)
    {
        _logger = logger;
        _service = service;
        _userManager = userManager;
        _themeService = themeService;
        _config = config;
    }

    private async Task<string> GetUser()
    {
        var result = await _userManager.FindByEmailAsync(User?.Identity?.Name!);
        return result.Id;
    }

    [OutputCache(Duration = 60)]
    public async Task<IActionResult> Index()
    {
        var apiKey = _config["SENDGRID_API_KEY"];
        // await EmailService.SendEmail(apiKey, "josephizang@prodigeenet.com", "joeizang@hotmail.com", "Testing sendgrid");
        var userId = await GetUser().ConfigureAwait(false);
        ViewBag.DayAverage = 3;
        ViewBag.Total = _service.GetExpenseTotalForLastMonth(userId);
        ViewBag.MeanTotalOver55Days = (await _service.GetMeanSpendByDays(userId, 7).ConfigureAwait(false))
            .ToString("F2");
        return View();
    }
}

