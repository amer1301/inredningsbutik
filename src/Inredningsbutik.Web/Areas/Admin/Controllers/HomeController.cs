using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Inredningsbutik.Infrastructure;

namespace Inredningsbutik.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AuthPolicies.AdminOnly)]
public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Index", "AdminHome", new { area = "Admin" });
}
