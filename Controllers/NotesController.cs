using Microsoft.AspNetCore.Mvc;

namespace NotesApi.Controllers;
[ApiController]
[Route("[controller]")]
public class NotesController : ControllerBase
{
    static List<Note> Notes = new();
    static int NextId = 1;

    [HttpGet(Name = "GetNotes")]
    public IEnumerable<Note> Get()
    {
        return Notes;
    }

    [HttpGet("{id}",Name = "GetNote")]
    public ActionResult<Note> Get(int id)
    {
        Note? note = Notes.FirstOrDefault(currentNote => currentNote.Id == id);

        if (note == null)
        {
            return NotFound();
        }

        return Ok(note);
    }

    [HttpPost(Name = "AddNote")]
    public ActionResult<Note> Post(CreateNoteRequest request)
    {
        Note note = new Note
        {
            Id = NextId++,
            Title = request.Title,
            Content = request.Content
        };
        Notes.Add(note);
        return Ok(note);
    }

    [HttpDelete("{id}", Name = "DeleteNote")]
    public ActionResult Delete(int id)
    {
        Note? note = Notes.FirstOrDefault(currentNote => currentNote.Id == id);

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
        Note? note = Notes.FirstOrDefault(currentNote => currentNote.Id == id);

        if (note == null)
        {
            return NotFound();
        }

        note.Title = request.Title;
        note.Content = request.Content;

        return Ok(note);
    }
}