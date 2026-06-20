using NotesApi.Dtos;
using NotesApi.Models;

namespace NotesApi.Services;
public interface INoteService
{
    IEnumerable<Note> GetAll(int userId);
    Note? GetById(int id, int userId);
    Note Create(CreateNoteRequest request, int userId);
    Note? Delete(int id, int userId);
    Note? Update(int id, UpdateNoteRequest request, int userId);
}
