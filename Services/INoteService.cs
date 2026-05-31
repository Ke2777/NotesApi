using NotesApi.Dtos;
using NotesApi.Models;

namespace NotesApi.Services;
public interface INoteService
{
    IEnumerable<Note> GetAll();
    Note? GetById(int id);
    Note Create(CreateNoteRequest request);
    Note? Delete(int id);
    Note? Update(int id, UpdateNoteRequest request);
}