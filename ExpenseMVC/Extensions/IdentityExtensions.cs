using ExpenseMVC.Models;
using Microsoft.AspNetCore.Identity;

namespace ExpenseMVC;

public static class IdentityExtensions
{
    public static Currency GetUserPreferredCurrency(this UserManager<ApplicationUser> userManager, ApplicationUser user) => user.PreferredCurrency;

    public static async Task<IdentityResult> SetUserPreferredCurrency(this UserManager<ApplicationUser> userManager, ApplicationUser user, Currency currency)
    {
        user.PreferredCurrency = currency;
        return await userManager.UpdateAsync(user).ConfigureAwait(false);
    }
}
