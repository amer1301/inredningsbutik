using Inredningsbutik.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Inredningsbutik.Web.Areas.Identity.Pages.Account.Manage
{
public class AddressModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AddressModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [TempData]
    public string? StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();


        public class InputModel
        {
            public string? Street { get; set; }
            public string? PostalCode { get; set; }
            public string? City { get; set; }
            public string? Country { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            Input = new InputModel
            {
                Street = user.Street,
                PostalCode = user.PostalCode,
                City = user.City,
                Country = user.Country
            };

            return Page();
        }

 public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        user.Street = Input.Street;
        user.PostalCode = Input.PostalCode;
        user.City = Input.City;
        user.Country = Input.Country;

        await _userManager.UpdateAsync(user);

        StatusMessage = "Adressen har sparats.";
        return RedirectToPage();
    }
}
}
