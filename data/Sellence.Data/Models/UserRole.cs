using Sellence.Data.Models.Bases;
using Sellence.Data.Models.Users;

namespace Sellence.Data.Models;

public sealed class UserRole : BaseEntity
{
    public Guid UserId { get; set; }

    public User? User { get; set; }

    public Guid RoleId { get; set; }

    public Role? Role { get; set; }
}