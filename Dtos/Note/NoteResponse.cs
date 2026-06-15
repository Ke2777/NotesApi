namespace NotesApi.Dtos;

public class NoteResponse
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
}