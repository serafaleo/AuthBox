using AuthBox.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthBox.Models.Models;

[Index(nameof(Email), nameof(Role), IsUnique = true)]
public class User
{
    [Key]
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required byte[] PasswordSalt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public required EUserRoles Role { get; set; }
}
