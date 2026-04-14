using ComicRealmBE.Models;
using ComicRealmBE.Services;
using Microsoft.AspNetCore.Mvc;

namespace ComicRealmBE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Replace this with your real DB lookup + password hash verification
        if (request.Username == "admin" && request.Password == "Password123!")
        {
            var user = new UserModel
            {
                Id = 1,
                Username = "admin",
                Role = Enums.UserRole.Admin
            };

            var token = _authService.GenerateJwtToken(user);

            return Ok(new
            {
                token,
                role = user.Role.ToString(),
                username = user.Username
            });
        }

        return Unauthorized(new { message = "Invalid username or password" });
    }
}

public class LoginRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}