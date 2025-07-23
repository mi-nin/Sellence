using Microsoft.EntityFrameworkCore;
using Sellence.Data.ModelBuilders;
using Sellence.Data.Models;
using Sellence.Data.Models.Users;

namespace Sellence.Data.Contexts;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.BuildUser();
        builder.BuildRole();
        builder.BuildUserRole();
        builder.BuildRolePermission();
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<UserRole> UserRoles { get; set; }

    public DbSet<RolePermission> RolePermissions { get; set; }
}
