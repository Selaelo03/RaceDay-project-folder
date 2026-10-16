
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Participant,Organiser")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db)
        {
            _db = db;
        }

        private int? GetCurrentUserId()
        {
            var value = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(value, out var id)
                ? id
                : null;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            var user = await _db.Users
                .AsNoTracking()
                .Where(u => u.UserID == userId.Value)
                .Select(u => new
                {
                    u.UserID,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Role,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }

        public class UpdateProfileRequest
        {
            [Required]
            [MaxLength(50)]
            public string FirstName { get; set; } = "";

            [Required]
            [MaxLength(50)]
            public string LastName { get; set; } = "";

            [Required]
            [EmailAddress]
            [MaxLength(100)]
            public string Email { get; set; } = "";
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileRequest request)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            var user = await _db.Users
                .FindAsync(userId.Value);

            if (user == null)
                return NotFound("User not found.");

            var emailExists = await _db.Users
                .AnyAsync(u =>
                    u.Email == request.Email &&
                    u.UserID != userId.Value);

            if (emailExists)
                return Conflict("Email already in use.");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                Message = "Profile updated successfully.",
                user.UserID,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role
            });
        }
    }
}
