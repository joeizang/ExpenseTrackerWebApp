using Microsoft.AspNetCore.Authentication.Cookies;

namespace ExpenseMVC;

internal sealed class CookieManagerWrapper : ICookieManager
{
    private const string LegacySuffix = "-$#legacy#$";
    private readonly ICookieManager _cookies = new ChunkingCookieManager();

    public string? GetRequestCookie(HttpContext context, string key)
    {
        // Try to read the normal cookie
        if (_cookies.GetRequestCookie(context, key) is { } value)
        {
            // We found it, all good
            return value;
        }

        // Try to get the legacy cookie instead
        return _cookies.GetRequestCookie(context, $"{key}{LegacySuffix}");
    }

    // https://andrewlock.net/supporting-legacy-browsers-and-samesite-cookies-without-useragent-sniffing-in-aspnetcore/
    public void AppendResponseCookie(HttpContext context, string key, string? value, CookieOptions options)
    {
        _cookies.AppendResponseCookie(context, key, value, options);
        
        //if the cookie we're setting is SameSite=None, set as legacy
        if (options.SameSite != SameSiteMode.None) return;
        // Create a copy of the options but remove SameSite setting by setting to Unspecified
        var clonedOptions = new CookieOptions(options)
        {
            SameSite = SameSiteMode.Unspecified
        };
        // Append legacySuffix to the cookie name and set the cookie
        _cookies.AppendResponseCookie(context, $"{key}{LegacySuffix}", value, clonedOptions);
    }

    public void DeleteCookie(HttpContext context, string key, CookieOptions options)
    {
        // Delete the normal cookie
        _cookies.DeleteCookie(context, key, options);

        // If the cookie we're setting is SameSite=None, delete the legacy cookie
        if (options.SameSite != SameSiteMode.None) return;
        // Create a copy of the options (HttpOnly, Secure etc)
        // but remove the SameSite setting by setting to Unspecified
        var clonedOptions = new CookieOptions(options)
        {
            SameSite = SameSiteMode.Unspecified
        };

        // Append the LegacySuffix to the cookie name, and delete the cookie
        _cookies.DeleteCookie(context, $"{key}{LegacySuffix}", clonedOptions);
    }
}