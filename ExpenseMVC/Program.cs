using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ExpenseMVC.Data;
using ExpenseMVC.Models;
using dotenv.net;
using ExpenseMVC;
using ExpenseMVC.BusinessLogicServices;
using ExpenseMVC.Validators;
using ExpenseMVC.ViewModels.ExpenseVM;
using FluentValidation;
using ExpenseMVC.BusinessLogicServices.ExpenseServiceLogic;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authentication.Cookies;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration["MYSQLDB"] 
                       ?? throw new InvalidOperationException("Connection string 'MYSQLDB' not found.");
var dataProtectString = builder.Configuration["MYSQLDataProtect"] 
                        ?? throw new InvalidOperationException("Connection string 'MYSQLDB' not found.");
builder.Services.AddDbContextPool<ApplicationDbContext>(options => {
    options.UseMySQL(connectionString);
    options.UseModel(ApplicationDbContextModel.Instance);
});
builder.Services.AddDbContext<DataProtectionContext>(options =>
{
    options.UseMySQL(dataProtectString);
    options.UseModel(DataProtectionContextModel.Instance);

});
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<DataProtectionContext>();


builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddSingleton(typeof(ThemeService));

builder.Services.AddScoped<IExpenseDataService, ExpenseDataService>();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => 
        options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.Configure<CookieAuthenticationOptions>(
    IdentityConstants.ApplicationScheme,
    scheme => scheme.Cookie.SameSite = SameSiteMode.None
);
builder.Services.Configure<CookieAuthenticationOptions>(
    IdentityConstants.ApplicationScheme,
    scheme => scheme.CookieManager = new CookieManagerWrapper()
);
builder.Services.AddAuthentication()
    .AddCookie(x =>
    {
        x.CookieManager = new CookieManagerWrapper();
        x.Cookie.SameSite = SameSiteMode.None;
        x.Cookie.HttpOnly = true;
        x.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddGoogle(opt => {
        opt.ClientId = builder.Configuration["CLIENT_ID"]!;
        opt.ClientSecret = builder.Configuration["CLIENT_SECRET"]!;
    });
builder.Services.AddScoped<IValidator<CreateExpenseViewModel>, CreateExpenseViewModelValidator>();
builder.Services.AddControllersWithViews();
builder.Services.AddOutputCache(options => {
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(30)));
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

app.Use((context, next) =>
{
    context.Request.Host = new HostString("fruglapp.prodigeenet.com");
    context.Request.Scheme = "https";
    return next();
});

// Configure the HTTP request pipeline.
app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseOutputCache();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();

