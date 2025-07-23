using Microsoft.EntityFrameworkCore;
using Sellence.Data.Models;

namespace Sellence.Data.ModelBuilders;

internal static class RolePermissionBuilder
{
    internal static void BuildRolePermission(this ModelBuilder builder)
    {
        builder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RolePermission>()
            .Property(rp => rp.Subject)
            .IsRequired();

        builder.Entity<RolePermission>()
            .Property(rp => rp.Actions)
            .IsRequired();

        builder.Entity<RolePermission>()
            .HasIndex(rp => new { rp.RoleId, rp.Subject })
            .IsUnique();
    }
}
