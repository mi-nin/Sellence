using Microsoft.EntityFrameworkCore;
using Sellence.Data.Models;

namespace Sellence.Data.ModelBuilders;

internal static class UserRoleBuilder
{
    internal static void BuildUserRole(this ModelBuilder builder)
    {
        builder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany()
            .HasForeignKey(ur => ur.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<UserRole>()
            .HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique();
    }
}
