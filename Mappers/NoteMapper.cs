using NotesApi.Dtos;
using NotesApi.Models;

namespace NotesApi.Mappers;

public static class NoteMapper
{
    public static NoteResponse ToNoteResponse(this Note note)
    {
        return new NoteResponse
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content
        };
    }
}