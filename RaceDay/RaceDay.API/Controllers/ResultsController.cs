
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
    public class ResultsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ResultsController(AppDbContext db)
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

        public class ResultRequest
        {
            [Required]
            public TimeSpan? FinishTime { get; set; }

            [Required]
            [Range(1, int.MaxValue)]
            public int FinishingPosition { get; set; }
        }

        // POST: api/enrollments/1/result
        [HttpPost("api/enrollments/{enrollmentId:int}/result")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> RecordResult(
            int enrollmentId,
            [FromBody] ResultRequest request)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var enrollment = await _db.Enrollments
                .Include(e => e.Event)
                .ThenInclude(e => e.Organiser)
                .FirstOrDefaultAsync(e =>
                    e.EnrollmentID == enrollmentId);

            if (enrollment == null)
                return NotFound("Enrollment not found.");

            if (enrollment.Event.Organiser.UserID != userId.Value)
                return Forbid();

            if (!request.FinishTime.HasValue ||
                request.FinishTime.Value <= TimeSpan.Zero ||
                request.FinishTime.Value >= TimeSpan.FromDays(1))
            {
                return BadRequest(new
                {
                    message =
                        "Finish time must be greater than zero and less than 24 hours."
                });
            }

            var existingResult = await _db.Results
                .AnyAsync(r =>
                    r.EnrollmentID == enrollmentId);

            if (existingResult)
            {
                return Conflict(new
                {
                    message =
                        "A result already exists for this enrollment."
                });
            }

            var result = new RaceDay.API.Models.Result
            {
                EnrollmentID = enrollmentId,
                FinishTime = request.FinishTime.Value,
                FinishingPosition = request.FinishingPosition
            };

            _db.Results.Add(result);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Result could not be recorded."
                });
            }

            return StatusCode(201, new
            {
                message = "Result recorded successfully.",
                result.ResultID,
                result.EnrollmentID,
                result.FinishTime,
                result.FinishingPosition
            });
        }

        // GET: api/users/profile/results
        [HttpGet("api/users/profile/results")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> GetMyResults()
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var participant = await _db.Participants
                .FirstOrDefaultAsync(p =>
                    p.UserID == userId.Value);

            if (participant == null)
                return Forbid();

            var results = await _db.Results
                .AsNoTracking()
                .Where(r =>
                    r.Enrollment.ParticipantID ==
                    participant.ParticipantID)
                .Select(r => new
                {
                    r.ResultID,
                    r.EnrollmentID,
                    r.FinishTime,
                    r.FinishingPosition,
                    r.Enrollment.EventID,
                    r.Enrollment.Event.EventName,
                    r.Enrollment.Event.EventDate,
                    r.Enrollment.Category.CategoryName
                })
                .ToListAsync();

            return Ok(results);
        }
    }
}
