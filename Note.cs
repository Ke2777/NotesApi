namespace NotesApi;

public class Note
{
    public int Id {get; set;}
    public required string Title {get; set;}
    public required string Content {get; set;}
}

public class CreateNoteRequest
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}