using NotesApi.Dtos;
using NotesApi.Models;

namespace NotesApi.Mappers;

public static class UserMapper
{
    public static UserResponse ToUserResponse(this User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            DateCreated = user.DateCreated
        };
    }
}
