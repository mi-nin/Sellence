using Sellence.Data.Models.Bases;
using Sellence.Shared.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Sellence.Data.Models.Users;

public abstract class User : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true)]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsEmailVerified { get; set; } = false;

    public bool IsActive { get; set; } = false;

    public DateTime? LastLoginAt { get; set; }

    public UserType Discriminator { get; set; } = UserType.Undefined;

    public string FullName => $"{FirstName} {LastName}";
}
