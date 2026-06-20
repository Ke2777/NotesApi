using NotesApi.Dtos;

namespace NotesApi.Services;

public interface IAuthService
{
    AuthResponse Register(CreateUserRequest request);
    AuthResponse? Login(LoginRequest request);
}
