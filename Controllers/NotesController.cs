using Microsoft.AspNetCore.Mvc;
using NotesApi.Mappers;
using NotesApi.Dtos;
using NotesApi.Models;
using NotesApi.Services;

namespace NotesApi.Controllers;

[ApiController]
[Route("notes")]
public class NotesController : ControllerBase
{
    private readonly INoteService noteService;

    public NotesController(INoteService noteService)
    {
        this.noteService = noteService;
    }

    [HttpGet(Name = "GetNotes")]
    public ActionResult<IEnumerable<NoteResponse>> Get()
    {
        return Ok(noteService.GetAll().Select(note => note.ToNoteResponse()));
    }

    [HttpGet("{id}", Name = "GetNote")]
    public ActionResult<NoteResponse> Get(int id)
    {
        Note? note = noteService.GetById(id);

        if (note == null)
        {
            return NotFound();
        }

        return Ok(note.ToNoteResponse());
    }

    [HttpPost(Name = "AddNote")]
    public ActionResult<NoteResponse> Post(CreateNoteRequest request)
    {
        Note note = noteService.Create(request);

        return CreatedAtAction(nameof(Get), new { id = note.Id }, note.ToNoteResponse());
    }

    [HttpDelete("{id}", Name = "DeleteNote")]
    public ActionResult Delete(int id)
    {
        Note? note = noteService.Delete(id);

        if (note == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{id}", Name = "UpdateNote")]
    public ActionResult<NoteResponse> Put(int id, UpdateNoteRequest request)
    {
        Note? note = noteService.Update(id, request);

        if (note == null)
        {
            return NotFound();
        }

        return Ok(note.ToNoteResponse());
    }
}