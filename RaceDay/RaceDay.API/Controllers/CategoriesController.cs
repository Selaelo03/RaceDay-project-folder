
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
    [Route("api/events/{eventId:int}/categories")]
    [Authorize(Roles = "Participant,Organiser")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CategoriesController(AppDbContext db)
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

        public class CategoryRequest
        {
            [Required]
            [MaxLength(100)]
            public string CategoryName { get; set; } = "";

            [Required]
            [MaxLength(50)]
            public string CategoryType { get; set; } = "";

            [Range(0, 150)]
            public int? MinimumAge { get; set; }

            [Range(0, 150)]
            public int? MaximumAge { get; set; }

            [Range(0.01, 99999999.99)]
            public decimal? Distance { get; set; }
        }

        // GET: api/events/1/categories
        [HttpGet]
        public async Task<IActionResult> GetCategories(int eventId)
        {
            var eventExists = await _db.Events
                .AnyAsync(e => e.EventID == eventId);

            if (!eventExists)
                return NotFound("Event not found.");

            var categories = await _db.Categories
                .AsNoTracking()
                .Where(c => c.EventID == eventId)
                .Select(c => new
                {
                    c.CategoryID,
                    c.EventID,
                    c.CategoryName,
                    c.CategoryType,
                    c.MinimumAge,
                    c.MaximumAge,
                    c.Distance
                })
                .ToListAsync();

            return Ok(categories);
        }

        // POST: api/events/1/categories
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateCategory(
            int eventId,
            [FromBody] CategoryRequest request)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var ev = await _db.Events
                .Include(e => e.Organiser)
                .FirstOrDefaultAsync(e => e.EventID == eventId);

            if (ev == null)
                return NotFound("Event not found.");

            if (ev.Organiser.UserID != userId.Value)
                return Forbid();

            if (request.MinimumAge.HasValue &&
                request.MaximumAge.HasValue &&
                request.MinimumAge > request.MaximumAge)
            {
                return BadRequest(
                    "Minimum age cannot exceed maximum age.");
            }

            var category = new Category
            {
                EventID = eventId,
                CategoryName = request.CategoryName.Trim(),
                CategoryType = request.CategoryType.Trim(),
                MinimumAge = request.MinimumAge,
                MaximumAge = request.MaximumAge,
                Distance = request.Distance
            };

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "Category created successfully.",
                category.CategoryID,
                category.EventID
            });
        }

        // PUT: api/events/1/categories/2
        [HttpPut("{categoryId:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateCategory(
            int eventId,
            int categoryId,
            [FromBody] CategoryRequest request)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var category = await _db.Categories
                .Include(c => c.Event)
                .ThenInclude(e => e.Organiser)
                .FirstOrDefaultAsync(c =>
                    c.CategoryID == categoryId &&
                    c.EventID == eventId);

            if (category == null)
                return NotFound("Category not found.");

            if (category.Event.Organiser.UserID != userId.Value)
                return Forbid();

            if (request.MinimumAge.HasValue &&
                request.MaximumAge.HasValue &&
                request.MinimumAge > request.MaximumAge)
            {
                return BadRequest(
                    "Minimum age cannot exceed maximum age.");
            }

            category.CategoryName = request.CategoryName.Trim();
            category.CategoryType = request.CategoryType.Trim();
            category.MinimumAge = request.MinimumAge;
            category.MaximumAge = request.MaximumAge;
            category.Distance = request.Distance;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Category updated successfully.",
                category.CategoryID
            });
        }

        // DELETE: api/events/1/categories/2
        [HttpDelete("{categoryId:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteCategory(
            int eventId,
            int categoryId)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var category = await _db.Categories
                .Include(c => c.Event)
                .ThenInclude(e => e.Organiser)
                .FirstOrDefaultAsync(c =>
                    c.CategoryID == categoryId &&
                    c.EventID == eventId);

            if (category == null)
                return NotFound("Category not found.");

            if (category.Event.Organiser.UserID != userId.Value)
                return Forbid();

            var hasEnrollments = await _db.Enrollments
                .AnyAsync(e => e.CategoryID == categoryId);

            if (hasEnrollments)
            {
                return Conflict(new
                {
                    message =
                        "Cannot delete a category with enrollments."
                });
            }

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
