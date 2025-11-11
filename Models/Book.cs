using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models;

public class Book
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public required string Title { get; set; }

    [Required]
    [StringLength(100)]
    public required string Author { get; set; }
    
    public string? ISBN { get; set; }
    public DateTime PublicationDate { get; set; }
}
