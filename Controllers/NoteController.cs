using Microsoft.AspNetCore.Mvc;

namespace NotesApi.Controllers;
[ApiController]
[Route("[controller]")]
public class NoteController : ControllerBase
{
    static List<Note> Notes = new();
    static int NextId = 1;

    [HttpGet(Name = "GetNotes")]
    public IEnumerable<Note> Get()
    {
        return Notes;
    }

    [HttpPost(Name = "AddNote")]
    public ActionResult<Note> Post(CreateNoteRequest request)
    {
        Note note = new Note
        {
            ID = NextId++,
            Title = request.Title,
            Body = request.Body
        };
        Notes.Add(note);
        return Ok(note);
    }

    [HttpDelete("{id}", Name = "DeleteNote")]
    public ActionResult Delete(int id)
    {
        Note? note = Notes.FirstOrDefault(currentNote => currentNote.ID == id);

        if (note == null)
        {
            return NotFound();
        }

        Notes.Remove(note);

        return NoContent();
    }

    [HttpPut("{id}", Name = "UpdateNote")]
    public ActionResult<Note> Put(int id, CreateNoteRequest request)
    {
        Note? note = Notes.FirstOrDefault(currentNote => currentNote.ID == id);

        if (note == null)
        {
            return NotFound();
        }

        note.Title = request.Title;
        note.Body = request.Body;

        return Ok(note);
    }
}