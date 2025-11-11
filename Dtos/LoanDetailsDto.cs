namespace LibraryApi.Dtos;

// DTO for the controller to return loan details
public record LoanDetailsDto
{
    public int Id { get; init; }
    public DateTime LoanDate { get; init; }
    public DateTime DueDate { get; init; }
    public DateTime? ReturnDate { get; init; }
    public int BookId { get; init; }
    public string BookTitle { get; init; } = string.Empty;
    public int UserId { get; init; }
    public string UserFullName { get; init; } = string.Empty;
}
