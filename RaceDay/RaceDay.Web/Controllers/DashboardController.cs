using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers;

public class DashboardController(RaceDayApiClient api) : Controller
{
    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
            return RedirectToAction("Login", "Account");
        ViewBag.Events = await api.GetAsync("api/events");
        ViewBag.Role = HttpContext.Session.GetString("Role");
        return View();
    }
}
