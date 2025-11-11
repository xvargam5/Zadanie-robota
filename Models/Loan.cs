namespace LibraryApi.Models;

public class Loan
{
    // Primary key for the Loan
    public int Id { get; set; }

    // Foreign keys
    public int BookId { get; set; }
    public int UserId { get; set; }

    // Loan data
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; } // The question mark means the value can be null

    // Navigation properties - "shortcuts" to the related objects
    public Book Book { get; set; } = null!;
    public User User { get; set; } = null!;
}
