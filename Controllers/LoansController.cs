using LibraryApi.Data;
using LibraryApi.Dtos;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly LibraryDbContext _db;

    public LoansController(LibraryDbContext db)
    {
        _db = db;
    }

    // POST: api/loans
    // Creates a new loan
    [HttpPost]
    [ProducesResponseType(typeof(LoanDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanDetailsDto>> CreateLoan(CreateLoanDto loanDto)
    {
        var book = await _db.Books.FindAsync(loanDto.BookId);
        if (book is null)
        {
            return NotFound($"Book with ID {loanDto.BookId} not found.");
        }

        var user = await _db.Users.FindAsync(loanDto.UserId);
        if (user is null)
        {
            return NotFound($"User with ID {loanDto.UserId} not found.");
        }

        var isAlreadyLoaned = await _db.Loans.AnyAsync(l => l.BookId == loanDto.BookId && l.ReturnDate == null);
        if (isAlreadyLoaned)
        {
            return BadRequest("This book is currently unavailable.");
        }

        var loan = new Loan
        {
            BookId = loanDto.BookId,
            UserId = loanDto.UserId,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            ReturnDate = null,
            Book = book,
            User = user
        };

        _db.Loans.Add(loan);
        await _db.SaveChangesAsync();

        var loanDetailsDto = new LoanDetailsDto
        {
            Id = loan.Id,
            LoanDate = loan.LoanDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            BookId = loan.BookId,
            BookTitle = loan.Book.Title,
            UserId = loan.UserId,
            UserFullName = $"{loan.User.FirstName} {loan.User.LastName}"
        };

        return Created($"/api/loans/{loan.Id}", loanDetailsDto);
    }

    // POST: api/loans/{id}/return
    // Marks an existing loan as returned
    [HttpPost("{id}/return")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReturnLoan(int id)
    {
        var loan = await _db.Loans.FindAsync(id);

        if (loan is null)
        {
            return NotFound("Loan not found.");
        }

        if (loan.ReturnDate is not null)
        {
            return BadRequest("This loan has already been returned.");
        }

        loan.ReturnDate = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
