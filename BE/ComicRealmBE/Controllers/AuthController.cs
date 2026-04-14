using ComicRealmBE.Data;
using ComicRealmBE.Dtos;
using ComicRealmBE.Models;
using ComicRealmBE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComicRealmBE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ComicRealmContext _context;
    private readonly AuthService _authService;

    public AuthController(ComicRealmContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive);

        if (user is null || user.PasswordHash != dto.PasswordHash)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var token = _authService.GenerateJwtToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            UserId = user.UserId,
            Email = user.Email,
            Role = user.Role.ToString()
        });
    }
}
