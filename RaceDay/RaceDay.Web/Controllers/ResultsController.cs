using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers;

public class ResultsController(RaceDayApiClient api) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("JwtToken") is null) return RedirectToAction("Login", "Account");
        if (HttpContext.Session.GetString("Role") != "Participant") return Forbid();
        ViewBag.Results = await api.GetAsync("api/users/profile/results");
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> EventEnrollments(int eventId)
    {
        if (HttpContext.Session.GetString("JwtToken") is null) return RedirectToAction("Login", "Account");
        if (HttpContext.Session.GetString("Role") != "Organiser") return Forbid();
        ViewBag.EventId = eventId;
        ViewBag.Enrollments = await api.GetAsync($"api/events/{eventId}/enrollments");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Record(int eventId, int enrollmentId, ResultViewModel model)
    {
        if (HttpContext.Session.GetString("JwtToken") is null) return RedirectToAction("Login", "Account");
        if (HttpContext.Session.GetString("Role") != "Organiser") return Forbid();
        if (!TimeSpan.TryParse(model.FinishTime, out var finish) || finish <= TimeSpan.Zero || finish >= TimeSpan.FromDays(1))
        {
            TempData["Error"] = "Enter a valid finish time, for example 00:32:15.";
            return RedirectToAction(nameof(EventEnrollments), new { eventId });
        }
        var result = await api.PostAsync($"api/enrollments/{enrollmentId}/result", new { FinishTime = finish, model.FinishingPosition });
        TempData[result.ok ? "Success" : "Error"] = result.ok ? "Result recorded successfully." : "Could not record result: " + result.message;
        return RedirectToAction(nameof(EventEnrollments), new { eventId });
    }
}
