using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inredningsbutik.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Index", "AdminHome", new { area = "Admin" });
}
