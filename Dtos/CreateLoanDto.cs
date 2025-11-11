using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Dtos;

public record CreateLoanDto(
    // Positive numbers
    [Range(1, int.MaxValue)] int BookId, 
    [Range(1, int.MaxValue)] int UserId);
