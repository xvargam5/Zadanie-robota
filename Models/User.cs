using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models;

public class User
{
    public int Id { get; set; }

    [StringLength(50)]
    public required string FirstName { get; set; }

    [StringLength(50)]
    public required string LastName { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; set; }
}
