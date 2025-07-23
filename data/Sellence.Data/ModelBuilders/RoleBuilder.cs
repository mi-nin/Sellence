using Microsoft.EntityFrameworkCore;
using Sellence.Data.Models;

internal static class RoleBuilder
{
    internal static void BuildRole(this ModelBuilder builder)
    {
        builder.Entity<Role>().HasIndex(r => r.Name).IsUnique();
    }
}
