using System.ComponentModel.DataAnnotations;

namespace NotesApi.Dtos;

public class UpdateNoteRequest
{
    [Required]
    [MinLength(3)]
    public required string Title { get; set; }
    [Required]
    [MaxLength(500)]
    public required string Content { get; set; }
}