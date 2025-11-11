using LibraryApi.Data;
using LibraryApi.Dtos;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure services
// Add Entity Framework Core DbContext
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services for API documentation (Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// Define API endpoints
// Create book endpoint
app.MapPost("/api/books", async (Book book, LibraryDbContext db) =>
{
    db.Books.Add(book);
    await db.SaveChangesAsync();

    return Results.Created($"/api/books/{book.Id}", book);
})
.Produces<Book>(StatusCodes.Status201Created);

// Get book endpoint
app.MapGet("/api/books/{id}", async (int id, LibraryDbContext db) =>
{
    var book = await db.Books.FindAsync(id);

    if (book is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(book);
})
.Produces<Book>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

// Update book endpoint
app.MapPut("/api/books/{id}", async (int id, Book updatedBook, LibraryDbContext db) =>
{
    var book = await db.Books.FindAsync(id);

    if (book is null)
    {
        return Results.NotFound();
    }

    book.Title = updatedBook.Title;
    book.Author = updatedBook.Author;
    book.ISBN = updatedBook.ISBN;
    book.PublicationDate = updatedBook.PublicationDate;

    await db.SaveChangesAsync();

    return Results.NoContent();
})
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

// Delete book endpoint
app.MapDelete("/api/books/{id}", async (int id, LibraryDbContext db) =>
{
    var book = await db.Books.FindAsync(id);

    if (book is null)
    {
        return Results.NotFound();
    }

    db.Books.Remove(book);
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

// Create user Endpoint - for testing purpose
app.MapPost("/api/users", async (User user, LibraryDbContext db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created($"/api/users/{user.Id}", user);
})
.Produces<User>(StatusCodes.Status201Created);

// Create loan endpoint
app.MapPost("/api/loans", async (CreateLoanDto loanDto, LibraryDbContext db) =>
{
    // Validation: Check if the book and user exist
    var book = await db.Books.FindAsync(loanDto.BookId);
    if (book is null)
    {
        return Results.NotFound($"Book with ID {loanDto.BookId} not found.");
    }

    var user = await db.Users.FindAsync(loanDto.UserId);
    if (user is null)
    {
        return Results.NotFound($"User with ID {loanDto.UserId} not found.");
    }

    // Validation: Check if the book is already loaned out
    var isAlreadyLoaned = await db.Loans.AnyAsync(l => l.BookId == loanDto.BookId && l.ReturnDate == null);
    if (isAlreadyLoaned)
    {
        return Results.BadRequest("This book is currently unavailable.");
    }

    var loan = new Loan
    {
        BookId = loanDto.BookId,
        UserId = loanDto.UserId,
        LoanDate = DateTime.UtcNow,
        DueDate = DateTime.UtcNow.AddDays(30),
        ReturnDate = null,
        
        // Assign the loaded entities
        Book = book,
        User = user
    };

    db.Loans.Add(loan);
    await db.SaveChangesAsync();

    // Create the DTO for the response
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

    return Results.Created($"/api/loans/{loan.Id}", loanDetailsDto);
})
.Produces<LoanDetailsDto>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status400BadRequest);

// Return loan Endpoint
app.MapPost("/api/loans/{id}/return", async (int id, LibraryDbContext db) =>
{
    var loan = await db.Loans.FindAsync(id);

    if (loan is null)
    {
        return Results.NotFound("Loan not found.");
    }

    if (loan.ReturnDate is not null)
    {
        return Results.BadRequest("This loan has already been returned.");
    }

    loan.ReturnDate = DateTime.UtcNow;
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status400BadRequest);

app.Run();
