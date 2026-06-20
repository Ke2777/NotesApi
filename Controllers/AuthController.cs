using Microsoft.AspNetCore.Mvc;
using NotesApi.Dtos;
using NotesApi.Services;

namespace NotesApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public ActionResult<AuthResponse> Register(CreateUserRequest request)
    {
        try
        {
            return Ok(_authService.Register(request));
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(exception.Message);
        }
    }

    [HttpPost("login")]
    public ActionResult<AuthResponse> Login(LoginRequest request)
    {
        AuthResponse? response = _authService.Login(request);

        if (response == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        return Ok(response);
    }
}
