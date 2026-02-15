using Microsoft.AspNetCore.Mvc;

namespace Inredningsbutik.Web.Controllers;

public class CustomerServiceController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
