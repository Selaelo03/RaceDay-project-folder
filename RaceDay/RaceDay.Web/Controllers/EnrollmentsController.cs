using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers;

public class EnrollmentsController(RaceDayApiClient api) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("JwtToken") is null) return RedirectToAction("Login", "Account");
        if (HttpContext.Session.GetString("Role") != "Participant") return Forbid();
        ViewBag.Enrollments = await api.GetAsync("api/users/profile/enrollments");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int eventId, int categoryId)
    {
        if (HttpContext.Session.GetString("JwtToken") is null) return RedirectToAction("Login", "Account");
        if (HttpContext.Session.GetString("Role") != "Participant") return Forbid();
        var result = await api.PostAsync($"api/events/{eventId}/enrollments", new { CategoryID = categoryId });
        TempData[result.ok ? "Success" : "Error"] = result.ok
            ? "You have successfully enrolled in this event."
            : "Enrollment failed: " + result.message;
        return RedirectToAction("Details", "Events", new { id = eventId });
    }
}
