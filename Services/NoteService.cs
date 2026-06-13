using Microsoft.EntityFrameworkCore;
using NotesApi.Data;
using NotesApi.Dtos;
using NotesApi.Models;

namespace NotesApi.Services;

public class NoteService : INoteService
{
    private readonly AppDbContext _dbContext;
    public NoteService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Note Create(CreateNoteRequest request)
    {
        Note note = new Note
        {
            Title = request.Title,
            Content = request.Content
        };

        _dbContext.Notes.Add(note);
        _dbContext.SaveChanges();

        return note;
    }

    public Note? Delete(int id)
    {
        Note? note = _dbContext.Notes.FirstOrDefault(currentNote => currentNote.Id == id);

        if (note == null)
        {
            return null;
        }

        _dbContext.Notes.Remove(note);
        _dbContext.SaveChanges();

        return note;
    }

    public IEnumerable<Note> GetAll()
    {
        return _dbContext.Notes;
    }

    public Note? GetById(int id)
    {
        return _dbContext.Notes.FirstOrDefault(currentNote => currentNote.Id == id);
    }

    public Note? Update(int id, UpdateNoteRequest request)
    {
        Note? note = _dbContext.Notes.FirstOrDefault(currentNote => currentNote.Id == id);

        if (note == null)
        {
            return null;
        }

        note.Title = request.Title;
        note.Content = request.Content;

        _dbContext.SaveChanges();
        return note;
    }
}
