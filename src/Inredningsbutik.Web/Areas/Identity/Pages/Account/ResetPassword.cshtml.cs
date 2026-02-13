using System.ComponentModel.DataAnnotations;
using Inredningsbutik.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Inredningsbutik.Web.Areas.Identity.Pages.Account;

public class ResetPasswordModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ResetPasswordModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "Lösenordet måste vara minst {2} tecken.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Nytt lösenord")]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [Compare("Password", ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmPassword { get; set; } = "";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.FindByEmailAsync(Input.Email);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Användaren hittades inte.");
            return Page();
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, token, Input.Password);

        if (result.Succeeded)
            return RedirectToPage("./Login");

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return Page();
    }
}
