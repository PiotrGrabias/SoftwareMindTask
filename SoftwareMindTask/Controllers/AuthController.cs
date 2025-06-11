using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoftwareMindTask.DTOs;
using SoftwareMindTask.Services;

/// <summary>
/// Handles shop-user authentication
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }
    /// <summary>
    /// Registers a new user based on the data from the request's body
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var result = await _authService.RegisterUserAsync(model);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        return Ok("User registered successfully.");
    }
    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var token = await _authService.LoginUserAsync(model);
        if (token == null)
            return Unauthorized("Invalid credentials.");
        return Ok(new { Token = token });
    }
}
