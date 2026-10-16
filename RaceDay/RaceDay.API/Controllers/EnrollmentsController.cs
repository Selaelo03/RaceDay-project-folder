
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
    [Authorize]
    public class EnrollmentsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public EnrollmentsController(AppDbContext db)
        {
            _db = db;
        }

        private int? GetUserId()
        {
            var claim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(claim, out var id)
                ? id : null;
        }

        public class EnrollmentRequest
        {
            [Required]
            [Range(1, int.MaxValue)]
            public int CategoryID { get; set; }
        }

        // POST: api/events/1/enrollments
        [HttpPost("api/events/{eventId:int}/enrollments")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> CreateEnrollment(
            int eventId,
            [FromBody] EnrollmentRequest request)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var participant = await _db.Participants
                .FirstOrDefaultAsync(p =>
                    p.UserID == userId.Value);

            if (participant == null)
                return Forbid();

            var ev = await _db.Events
                .FirstOrDefaultAsync(e =>
                    e.EventID == eventId);

            if (ev == null)
                return NotFound("Event not found.");

            var category = await _db.Categories
                .FirstOrDefaultAsync(c =>
                    c.CategoryID == request.CategoryID &&
                    c.EventID == eventId);

            if (category == null)
            {
                return BadRequest(new
                {
                    message =
                        "Category does not belong to this event."
                });
            }

            var alreadyEnrolled = await _db.Enrollments
                .AnyAsync(e =>
                    e.ParticipantID == participant.ParticipantID &&
                    e.EventID == eventId);

            if (alreadyEnrolled)
            {
                return Conflict(new
                {
                    message =
                        "You are already enrolled in this event."
                });
            }

            var enrollment = new Enrollment
            {
                ParticipantID = participant.ParticipantID,
                EventID = eventId,
                CategoryID = request.CategoryID,
                EnrollmentDate = DateTime.Now
            };

            _db.Enrollments.Add(enrollment);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message =
                        "Enrollment could not be completed."
                });
            }

            return StatusCode(201, new
            {
                message = "Enrollment successful.",
                enrollment.EnrollmentID,
                enrollment.EventID,
                enrollment.CategoryID
            });
        }

        // GET: api/users/profile/enrollments
        [HttpGet("api/users/profile/enrollments")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var participant = await _db.Participants
                .FirstOrDefaultAsync(p =>
                    p.UserID == userId.Value);

            if (participant == null)
                return Forbid();

            var enrollments = await _db.Enrollments
                .AsNoTracking()
                .Where(e =>
                    e.ParticipantID == participant.ParticipantID)
                .Select(e => new
                {
                    e.EnrollmentID,
                    e.EnrollmentDate,
                    e.EventID,
                    e.Event.EventName,
                    e.Event.EventDate,
                    e.Event.Location,
                    e.CategoryID,
                    e.Category.CategoryName
                })
                .ToListAsync();

            return Ok(enrollments);
        }

        // GET: api/events/1/enrollments
        [HttpGet("api/events/{eventId:int}/enrollments")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> GetEventEnrollments(
            int eventId)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var ev = await _db.Events
                .Include(e => e.Organiser)
                .FirstOrDefaultAsync(e =>
                    e.EventID == eventId);

            if (ev == null)
                return NotFound("Event not found.");

            if (ev.Organiser.UserID != userId.Value)
                return Forbid();

            var enrollments = await _db.Enrollments
                .AsNoTracking()
                .Where(e => e.EventID == eventId)
                .Select(e => new
                {
                    e.EnrollmentID,
                    e.EnrollmentDate,
                    e.ParticipantID,
                    e.Participant.User.FirstName,
                    e.Participant.User.LastName,
                    e.CategoryID,
                    e.Category.CategoryName
                })
                .ToListAsync();

            return Ok(enrollments);
        }
    }
}
