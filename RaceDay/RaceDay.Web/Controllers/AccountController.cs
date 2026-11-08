using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;
namespace RaceDay.Web.Controllers;
public class AccountController(RaceDayApiClient api):Controller
{
 [HttpGet] public IActionResult Login()=>View();
 [HttpPost] public async Task<IActionResult> Login(LoginViewModel m){if(!ModelState.IsValid)return View(m);var r=await api.PostAsync("api/auth/login",new{m.Email,m.Password});if(!r.ok){ModelState.AddModelError("","Login failed. Check your details and API connection.");return View(m);}var d=r.data!.Value;HttpContext.Session.SetString("JwtToken",d.GetProperty("token").GetString()!);var u=d.GetProperty("user");HttpContext.Session.SetString("UserName",u.GetProperty("firstName").GetString()+" "+u.GetProperty("lastName").GetString());HttpContext.Session.SetString("Role",u.GetProperty("role").GetString()!);return RedirectToAction("Index","Dashboard");}
 [HttpGet] public IActionResult Register()=>View();
 [HttpPost] public async Task<IActionResult> Register(RegisterViewModel m){if(!ModelState.IsValid)return View(m);var r=await api.PostAsync("api/auth/register",m);if(!r.ok){ModelState.AddModelError("", "Registration failed: "+r.message);return View(m);}TempData["Success"]="Registration successful. Please log in.";return RedirectToAction(nameof(Login));}
 public IActionResult Logout(){HttpContext.Session.Clear();return RedirectToAction("Index","Home");}
}
