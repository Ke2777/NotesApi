using System.ComponentModel.DataAnnotations;

namespace NotesApi;

public class Note
{
    public int Id {get; set;}
    public required string Title {get; set;}
    public required string Content {get; set;}
}

public class CreateNoteRequest
{
    [Required]
    [MinLength(3)]
    public required string Title { get; set; }
    [Required]
    [MaxLength(500)]
    public required string Content { get; set; }
}