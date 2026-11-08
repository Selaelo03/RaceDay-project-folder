using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models;

public class LoginViewModel
{
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required] public string Password { get; set; } = "";
}

public class RegisterViewModel
{
    [Required] public string FirstName { get; set; } = "";
    [Required] public string LastName { get; set; } = "";
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required, MinLength(8)] public string Password { get; set; } = "";
    [Required] public string Role { get; set; } = "Participant";
    [Required] public string PhoneNumber { get; set; } = "";
    public DateTime? DateOfBirth { get; set; }
    public string? OrganisationName { get; set; }
}

public class EventViewModel
{
    public int EventID { get; set; }
    [Required, StringLength(100)] public string EventName { get; set; } = "";
    [StringLength(500)] public string? Description { get; set; }
    [Required] public DateTime EventDate { get; set; } = DateTime.Today.AddDays(7);
    [Required, StringLength(200)] public string Location { get; set; } = "";
    [Range(0.01, 99999999.99)] public decimal Distance { get; set; } = 5;
    [Required, StringLength(50)] public string EventType { get; set; } = "Road Running";
}

public class CategoryViewModel
{
    [Required, StringLength(100)] public string CategoryName { get; set; } = "";
    [Required, StringLength(50)] public string CategoryType { get; set; } = "Open";
    [Range(0, 150)] public int? MinimumAge { get; set; }
    [Range(0, 150)] public int? MaximumAge { get; set; }
    [Range(0.01, 99999999.99)] public decimal? Distance { get; set; }
}

public class ResultViewModel
{
    [Required] public string FinishTime { get; set; } = "00:30:00";
    [Range(1, int.MaxValue)] public int FinishingPosition { get; set; } = 1;
}
