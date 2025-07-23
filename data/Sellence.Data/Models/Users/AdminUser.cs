using Sellence.Shared.Models.Enums;

namespace Sellence.Data.Models.Users;

public sealed class AdminUser : User
{
    public Guid? CreatedByAdminId { get; set; }

    public AdminUser? CreatedByAdmin { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];

    public AdminUser()
    {
        this.Discriminator = UserType.AdminUser;
    }
}
