using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestAuthController : ControllerBase
    {
        [HttpGet("participant")]
        [Authorize(Roles = "Participant")]
        public IActionResult ParticipantOnly()
        {
            return Ok(new
            {
                Message = "Welcome Participant! You have access to this protected endpoint."
            });
        }
    }
}
