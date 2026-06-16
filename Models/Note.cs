namespace NotesApi.Models;

public class Note
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }
}