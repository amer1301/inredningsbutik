using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Inredningsbutik.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Inredningsbutik.Web.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        SignInManager<ApplicationUser> signInManager,
        ILogger<RegisterModel> logger)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string ReturnUrl { get; set; } = "/";

    public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "Lösenordet måste vara minst {2} och max {1} tecken.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [Compare("Password", ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmPassword { get; set; } = "";
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
{
    returnUrl ??= Url.Content("~/");
    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

    if (!ModelState.IsValid)
        return Page();

    var user = CreateUser();

    await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

    var result = await _userManager.CreateAsync(user, Input.Password);

    if (result.Succeeded)
    {
        _logger.LogInformation("Användare skapade nytt konto med lösenord.");

        // Lägg nya användare i Customer-rollen 
        await _userManager.AddToRoleAsync(user, "Customer");

        // RequireConfirmedAccount = false, så vi loggar in direkt:
        await _signInManager.SignInAsync(user, isPersistent: false);

        return LocalRedirect(returnUrl);
    }

    foreach (var error in result.Errors)
    {
        ModelState.AddModelError(string.Empty, error.Description);
    }

    return Page();
}


    private ApplicationUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>()!;
        }
        catch
        {
            throw new InvalidOperationException($"Kan inte skapa en instans av '{nameof(ApplicationUser)}'. " +
                                                $"Kontrollera att det har en parameterlös konstruktor.");
        }
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("Standardgränssnittet kräver en användarbutik med e-postsupport.");
        }
        return (IUserEmailStore<ApplicationUser>)_userStore;
    }
}
