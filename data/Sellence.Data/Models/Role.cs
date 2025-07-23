using Microsoft.EntityFrameworkCore;
using Sellence.Data.Models.Bases;
using System.ComponentModel.DataAnnotations;

namespace Sellence.Data.Models;

public sealed class Role : BaseEntity
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public ICollection<UserRole> UserRoles { get; set; } = [];

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}