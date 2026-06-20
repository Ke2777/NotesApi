using System.Security.Claims;
using NotesApi.Models;

namespace NotesApi.Services;

public interface IJwtTokenService
{
    string CreateToken(User user);
    ClaimsPrincipal? ValidateToken(string token);
}
