
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {

        private readonly AppDbContext _db;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthController(
            AppDbContext db,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            var role = request.Role?.Trim();

            if (role != "Participant" && role != "Organiser")
            {
                return BadRequest(new
                {
                    message = "Role must be Participant or Organiser."
                });
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return BadRequest(new
                {
                    message = "Phone number is required."
                });
            }

            if (role == "Participant" && request.DateOfBirth == null)
            {
                return BadRequest(new
                {
                    message = "Date of birth is required for Participants."
                });
            }

            if (role == "Organiser" &&
                string.IsNullOrWhiteSpace(request.OrganisationName))
            {
                return BadRequest(new
                {
                    message = "Organisation name is required for Organisers."
                });
            }

            var email = request.Email.Trim();

            if (await _db.Users.AnyAsync(u => u.Email == email))
            {
                return Conflict(new
                {
                    message = "Email address is already registered."
                });
            }

            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                Role = role
            };

            user.PasswordHash =
                _passwordHasher.HashPassword(user, request.Password);

            if (role == "Participant")
            {
                var participant = new Participant
                {
                    User = user,
                    DateOfBirth = request.DateOfBirth!.Value.Date,
                    PhoneNumber = request.PhoneNumber.Trim()
                };

                _db.Participants.Add(participant);
            }
            else
            {
                var organiser = new Organiser
                {
                    User = user,
                    OrganisationName = request.OrganisationName!.Trim(),
                    PhoneNumber = request.PhoneNumber.Trim()
                };

                _db.Organisers.Add(organiser);
            }

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Registration could not be completed."
                });
            }

            return StatusCode(201, new
            {
                message = $"{role} registered successfully.",
                userId = user.UserID,
                role = user.Role
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            var email = request.Email.Trim();

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(
                    user, request.Password);

                await _db.SaveChangesAsync();
            }

            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT key is missing.");

            var key = new SymmetricSecurityKey(
                Convert.FromBase64String(jwtKey));

            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var expiryMinutes = int.Parse(
                _configuration["Jwt:ExpiryMinutes"] ?? "60");

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return Ok(new
            {
                message = "Login successful.",
                token = tokenString,
                expiresIn = expiryMinutes * 60,
                user = new
                {
                    user.UserID,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Role
                }
            });
        }

    }
}
