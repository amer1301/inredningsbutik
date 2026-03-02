using Inredningsbutik.Infrastructure.Data;
using Inredningsbutik.Infrastructure.Identity;
using Inredningsbutik.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Inredningsbutik.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Inredningsbutik.Core.Interfaces;
using Inredningsbutik.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddRazorPages();

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// ---------- DATABASE PATH (Azure safe) ----------

var home = Environment.GetEnvironmentVariable("HOME");

string dbPath;

if (!string.IsNullOrEmpty(home))
{
    dbPath = Path.Combine(home, "site", "data", "inredningsbutik.db");
    Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
}
else
{
    dbPath = Path.Combine(builder.Environment.ContentRootPath, "inredningsbutik.db");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// ---------- IDENTITY ----------

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;
        options.Stores.MaxLengthForKeys = 450;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthPolicies.AdminOnly, policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy(AuthPolicies.SignedInUser, policy =>
        policy.RequireAuthenticatedUser());

    options.AddPolicy(AuthPolicies.CustomerOrAdmin, policy =>
        policy.RequireRole("Customer", "Admin"));
});

var app = builder.Build();

// ---------- MIDDLEWARE ----------

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// ---------- MIGRATION + SEED (SAFE VERSION) ----------

using (var scope = app.Services.CreateScope())
{
    try
    {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<AppDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        context.Database.Migrate();

        await IdentitySeeder.SeedAsync(roleManager, userManager);
        await DataSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Startup error:");
        Console.WriteLine(ex);
    }
}

app.Run();