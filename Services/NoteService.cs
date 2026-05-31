using NotesApi.Dtos;
using NotesApi.Models;
using System.Text.Json;

namespace NotesApi.Services;
public class NoteService : INoteService
{
    private List<Note> _notes = new();
    private int _nextId = 1;

    public NoteService()
{
    string path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "notes.json");

    if (!File.Exists(path))
    {
        _notes = new List<Note>();
        return;
    }

    string json = File.ReadAllText(path);

    if (string.IsNullOrWhiteSpace(json))
    {
        _notes = new List<Note>();
        return;
    }

    _notes = JsonSerializer.Deserialize<List<Note>>(json, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    }) ?? new List<Note>();

    _nextId = _notes.Count == 0 ? 1 : _notes.Max(note => note.Id) + 1;
}

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