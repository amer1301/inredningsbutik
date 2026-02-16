using Microsoft.AspNetCore.Mvc;

namespace Inredningsbutik.Web.Controllers;

public class CustomerServiceController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
