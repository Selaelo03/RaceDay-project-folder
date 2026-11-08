using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers;

public class EventsController(RaceDayApiClient api) : Controller
{
    private string? Role => HttpContext.Session.GetString("Role");
    private bool LoggedIn => !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("JwtToken"));

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var eventData = await api.GetAsync($"api/events/{id}");
        if (eventData is null || eventData.Value.ValueKind != JsonValueKind.Object)
            return NotFound("The event could not be found. Please return to the events list and try again.");

        ViewBag.Event = eventData.Value;
        ViewBag.Categories = LoggedIn && (Role == "Participant" || Role == "Organiser")
            ? await api.GetAsync($"api/events/{id}/categories")
            : null;
        ViewBag.Role = Role;
        return View();
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!LoggedIn || Role != "Organiser") return Forbid();
        return View(new EventViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EventViewModel model)
    {
        if (!LoggedIn || Role != "Organiser") return Forbid();
        if (!ModelState.IsValid) return View(model);
        var result = await api.PostAsync("api/events", model);
        if (!result.ok)
        {
            ModelState.AddModelError("", "Could not create event: " + result.message);
            return View(model);
        }
        TempData["Success"] = "Event created successfully. Add its race categories from the event details page.";
        if (result.data.HasValue && result.data.Value.ValueKind == JsonValueKind.Object &&
            result.data.Value.TryGetProperty("eventID", out var createdId))
            return RedirectToAction(nameof(Details), new { id = createdId.GetInt32() });
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!LoggedIn || Role != "Organiser") return Forbid();
        var data = await api.GetAsync($"api/events/{id}");
        if (data is null) return NotFound();
        var e = data.Value;
        return View(new EventViewModel
        {
            EventID = e.GetProperty("eventID").GetInt32(),
            EventName = e.GetProperty("eventName").GetString() ?? "",
            Description = e.TryGetProperty("description", out var d) ? d.GetString() : null,
            EventDate = e.GetProperty("eventDate").GetDateTime(),
            Location = e.GetProperty("location").GetString() ?? "",
            Distance = e.GetProperty("distance").GetDecimal(),
            EventType = e.GetProperty("eventType").GetString() ?? ""
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EventViewModel model)
    {
        if (!LoggedIn || Role != "Organiser") return Forbid();
        if (!ModelState.IsValid) return View(model);
        var result = await api.SendAsync(HttpMethod.Put, $"api/events/{id}", model);
        if (!result.ok)
        {
            ModelState.AddModelError("", "Could not update event: " + result.message);
            return View(model);
        }
        TempData["Success"] = "Event updated successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!LoggedIn || Role != "Organiser") return Forbid();
        var result = await api.DeleteAsync($"api/events/{id}");
        TempData[result.ok ? "Success" : "Error"] = result.ok
            ? "Event deleted successfully."
            : "Could not delete event: " + result.message;
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(int id, CategoryViewModel model)
    {
        if (!LoggedIn || Role != "Organiser") return Forbid();
        if (!ModelState.IsValid) return RedirectToAction(nameof(Details), new { id });
        var result = await api.PostAsync($"api/events/{id}/categories", model);
        TempData[result.ok ? "Success" : "Error"] = result.ok
            ? "Race category added successfully."
            : "Could not add category: " + result.message;
        return RedirectToAction(nameof(Details), new { id });
    }
}
