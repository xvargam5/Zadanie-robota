using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")] // This will make the base route /api/books
public class BooksController : ControllerBase
{
    private readonly LibraryDbContext _db;

    // The DbContext is injected via the constructor
    public BooksController(LibraryDbContext db)
    {
        _db = db;
    }

    // GET: api/books/{id}
    // Gets a specific book by its ID
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        var book = await _db.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound(); 
        }

        return Ok(book);
    }

    // POST: api/books
    // Creates a new book
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Book>> PostBook(Book book)
    {
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }

    // PATCH: api/books/{id}
    // Partially updates an existing book
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchBook(int id, [FromBody] JsonPatchDocument<Book> patchDoc)
    {
        if (patchDoc is null)
        {
            return BadRequest();
        }

        var book = await _db.Books.FindAsync(id);
        if (book is null)
        {
            return NotFound();
        }

        patchDoc.ApplyTo(book); // Apply the patch first

        // Manually trigger validation
        if (!TryValidateModel(book))
        {
            return ValidationProblem(ModelState);
        }

        await _db.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/books/{id}
    // Deletes a book
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _db.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        _db.Books.Remove(book);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
