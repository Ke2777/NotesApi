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

    public Note Create(CreateNoteRequest request, int userId)
    {
        Note note = new Note
        {
            Title = request.Title,
            Content = request.Content,
            UserId = userId
        };

        _dbContext.Notes.Add(note);
        _dbContext.SaveChanges();

        return note;
    }

    public Note? Delete(int id, int userId)
    {
        Note? note = _dbContext.Notes.FirstOrDefault(currentNote => currentNote.Id == id && currentNote.UserId == userId);

        if (note == null)
        {
            return null;
        }

        _dbContext.Notes.Remove(note);
        _dbContext.SaveChanges();

        return note;
    }

    public IEnumerable<Note> GetAll(int userId)
    {
        return _dbContext.Notes.Where(note => note.UserId == userId);
    }

    public Note? GetById(int id, int userId)
    {
        return _dbContext.Notes.FirstOrDefault(currentNote => currentNote.Id == id && currentNote.UserId == userId);
    }

    public Note? Update(int id, UpdateNoteRequest request, int userId)
    {
        Note? note = _dbContext.Notes.FirstOrDefault(currentNote => currentNote.Id == id && currentNote.UserId == userId);

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
