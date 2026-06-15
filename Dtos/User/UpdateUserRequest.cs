using System.ComponentModel.DataAnnotations;

namespace NotesApi.Dtos;

public class UpdateUserRequest
{
    [Required]
    [MinLength(3)]
    public required string Username { get; set; }
    [Required]
    [MaxLength(500)]
    public required string Email { get; set; }
    [Required]
    [MinLength(6)]
    public required string Password { get; set; }
}