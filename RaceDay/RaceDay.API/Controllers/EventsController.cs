
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/events")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public EventsController(AppDbContext db)
        {
            _db = db;
        }

        private int? GetUserId()
        {
            var claim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(claim, out int id)
                ? id : null;
        }

        public class EventRequest
        {
            [Required]
            [MaxLength(100)]
            public string EventName { get; set; } = "";

            [MaxLength(500)]
            public string? Description { get; set; }

            [Required]
            public DateTime EventDate { get; set; }

            [Required]
            [MaxLength(200)]
            public string Location { get; set; } = "";

            [Range(0.01, 99999999.99)]
            public decimal Distance { get; set; }

            [Required]
            [MaxLength(50)]
            public string EventType { get; set; } = "";
        }

        // GET: api/events
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _db.Events
                .AsNoTracking()
                .Select(e => new
                {
                    e.EventID,
                    e.EventName,
                    e.Description,
                    e.EventDate,
                    e.Location,
                    e.Distance,
                    e.EventType,
                    e.OrganiserID
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/events/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var ev = await _db.Events
                .AsNoTracking()
                .Where(e => e.EventID == id)
                .Select(e => new
                {
                    e.EventID,
                    e.EventName,
                    e.Description,
                    e.EventDate,
                    e.Location,
                    e.Distance,
                    e.EventType,
                    e.OrganiserID
                })
                .FirstOrDefaultAsync();

            if (ev == null)
                return NotFound("Event not found.");

            return Ok(ev);
        }

        // POST: api/events
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateEvent(
            [FromBody] EventRequest request)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var organiser = await _db.Organisers
                .FirstOrDefaultAsync(o =>
                    o.UserID == userId.Value);

            if (organiser == null)
                return Forbid();

            var ev = new RaceDay.API.Models.Event
            {
                OrganiserID = organiser.OrganiserID,
                EventName = request.EventName,
                Description = request.Description,
                EventDate = request.EventDate,
                Location = request.Location,
                Distance = request.Distance,
                EventType = request.EventType
            };

            _db.Events.Add(ev);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = ev.EventID },
                new { ev.EventID, ev.EventName });
        }

        // PUT: api/events/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            [FromBody] EventRequest request)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var organiser = await _db.Organisers
                .FirstOrDefaultAsync(o =>
                    o.UserID == userId.Value);

            if (organiser == null)
                return Forbid();

            var ev = await _db.Events.FindAsync(id);

            if (ev == null)
                return NotFound("Event not found.");

            if (ev.OrganiserID != organiser.OrganiserID)
                return Forbid();

            ev.EventName = request.EventName;
            ev.Description = request.Description;
            ev.EventDate = request.EventDate;
            ev.Location = request.Location;
            ev.Distance = request.Distance;
            ev.EventType = request.EventType;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                Message = "Event updated successfully.",
                ev.EventID
            });
        }

        // DELETE: api/events/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var organiser = await _db.Organisers
                .FirstOrDefaultAsync(o =>
                    o.UserID == userId.Value);

            if (organiser == null)
                return Forbid();

            var ev = await _db.Events.FindAsync(id);

            if (ev == null)
                return NotFound("Event not found.");

            if (ev.OrganiserID != organiser.OrganiserID)
                return Forbid();

            _db.Events.Remove(ev);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
