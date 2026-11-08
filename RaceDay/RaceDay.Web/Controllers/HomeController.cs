using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers;

public class HomeController(RaceDayApiClient api) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.Events = await api.GetAsync("api/events");
        return View();
    }
    public IActionResult Privacy() => View();
}
