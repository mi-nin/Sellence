using Microsoft.EntityFrameworkCore;
using Sellence.Data.Models.Users;
using Sellence.Shared.Models.Enums;
using System.Reflection.Emit;

namespace Sellence.Data.ModelBuilders;

internal static class UserBuilder
{
    internal static void BuildUser(this ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasDiscriminator<UserType>("Discriminator")
            .HasValue<AdminUser>(UserType.AdminUser)
            .HasValue<ShopUser>(UserType.ShopUser)
            .HasValue<User>(UserType.Undefined);

        builder.Entity<User>().Property(u => u.UserName).IsRequired().HasMaxLength(100);
        builder.Entity<User>().Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }
}
