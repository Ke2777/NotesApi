using NotesApi.Dtos;
using NotesApi.Models;

namespace NotesApi.Services;
public class NoteService : INoteService
{
    private List<Note> _notes = new();
    private int _nextId = 1;

    public Note Create(CreateNoteRequest request)
    {
        Note note = new Note
        {
            Id = _nextId++,
            Title = request.Title,
            Content = request.Content
         };
        _notes.Add(note);
        return note;
    }

    public Note? Delete(int id)
    {
        Note? note = _notes.FirstOrDefault(currentNote => currentNote.Id == id);

        if (note == null)
        {
            return null;
        }

        _notes.Remove(note);

        return note;
    }

    public IEnumerable<Note> GetAll()
    {
        return _notes;
    }

    public Note? GetById(int id)
    {
        return _notes.FirstOrDefault(currentNote => currentNote.Id == id);
    }

    public Note? Update(int id, UpdateNoteRequest request)
    {
        Note? note = _notes.FirstOrDefault(currentNote => currentNote.Id == id);

        if (note == null)
        {
            return null;
        }

        note.Title = request.Title;
        note.Content = request.Content;

        return note;
    }
}