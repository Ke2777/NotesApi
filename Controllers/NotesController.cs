using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using NotesApi.Mappers;
using NotesApi.Dtos;
using NotesApi.Filters;
using NotesApi.Models;
using NotesApi.Services;

namespace NotesApi.Controllers;

[ApiController]
[RequireUser]
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
        int userId = GetCurrentUserId();

        return Ok(noteService.GetAll(userId).Select(note => note.ToNoteResponse()));
    }

    [HttpGet("{id}", Name = "GetNote")]
    public ActionResult<NoteResponse> Get(int id)
    {
        int userId = GetCurrentUserId();
        Note? note = noteService.GetById(id, userId);

        if (note == null)
        {
            return NotFound();
        }

        return Ok(note.ToNoteResponse());
    }

    [HttpPost(Name = "AddNote")]
    public ActionResult<NoteResponse> Post(CreateNoteRequest request)
    {
        int userId = GetCurrentUserId();
        Note note = noteService.Create(request, userId);

        return CreatedAtAction(nameof(Get), new { id = note.Id }, note.ToNoteResponse());
    }

    [HttpDelete("{id}", Name = "DeleteNote")]
    public ActionResult Delete(int id)
    {
        int userId = GetCurrentUserId();
        Note? note = noteService.Delete(id, userId);

        if (note == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{id}", Name = "UpdateNote")]
    public ActionResult<NoteResponse> Put(int id, UpdateNoteRequest request)
    {
        int userId = GetCurrentUserId();
        Note? note = noteService.Update(id, request, userId);

        if (note == null)
        {
            return NotFound();
        }

        return Ok(note.ToNoteResponse());
    }

    private int GetCurrentUserId()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userId, out int parsedUserId))
        {
            throw new UnauthorizedAccessException("Invalid user token.");
        }

        return parsedUserId;
    }
}
