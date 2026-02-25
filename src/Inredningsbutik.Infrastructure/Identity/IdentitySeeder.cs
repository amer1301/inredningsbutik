using Microsoft.AspNetCore.Identity;

namespace Inredningsbutik.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        string[] roles = ["Admin", "Customer"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = "admin@inredningsbutik.local";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin12345!");
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Kunde inte skapa admin: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        var customerEmail = "customer@inredningsbutik.local";
var customerUser = await userManager.FindByEmailAsync(customerEmail);

if (customerUser is null)
{
    customerUser = new ApplicationUser
    {
        UserName = customerEmail,
        Email = customerEmail,
        EmailConfirmed = true
    };

    var result = await userManager.CreateAsync(customerUser, "Customer123!");

    if (!result.Succeeded)
    {
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        throw new Exception($"Kunde inte skapa customer: {errors}");
    }
}

if (!await userManager.IsInRoleAsync(customerUser, "Customer"))
{
    await userManager.AddToRoleAsync(customerUser, "Customer");
}
    }
}
