using Microsoft.AspNetCore.Identity;
using NotesApi.Data;
using NotesApi.Dtos;
using NotesApi.Mappers;
using NotesApi.Models;

namespace NotesApi.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly PasswordHasher<object> _hasher = new();

    public AuthService(AppDbContext dbContext, IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
    }

    public AuthResponse Register(CreateUserRequest request)
    {
        string normalizedEmail = request.Email.Trim().ToLower();

        if (_dbContext.Users.Any(user => user.Email.ToLower() == normalizedEmail))
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        User user = new()
        {
            Username = request.Username.Trim(),
            Email = normalizedEmail,
            PasswordHash = _hasher.HashPassword(null!, request.Password),
            DateCreated = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();

        return CreateAuthResponse(user);
    }

    public AuthResponse? Login(LoginRequest request)
    {
        string normalizedEmail = request.Email.Trim().ToLower();
        User? user = _dbContext.Users.FirstOrDefault(currentUser => currentUser.Email.ToLower() == normalizedEmail);

        if (user == null)
        {
            return null;
        }

        PasswordVerificationResult result = _hasher.VerifyHashedPassword(null!, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return CreateAuthResponse(user);
    }

    private AuthResponse CreateAuthResponse(User user)
    {
        return new AuthResponse
        {
            Token = _jwtTokenService.CreateToken(user),
            User = user.ToUserResponse()
        };
    }
}
