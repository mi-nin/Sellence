using Sellence.Data.Models.Bases;
using Sellence.Shared.Models.Enums;


namespace Sellence.Data.Models;

public sealed class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public PermissionSubject Subject { get; set; }

    public PermissionAction Actions { get; set; }
}