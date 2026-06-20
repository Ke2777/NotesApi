namespace NotesApi.Dtos;

public class AuthResponse
{
    public required string Token { get; set; }
    public required UserResponse User { get; set; }
}
