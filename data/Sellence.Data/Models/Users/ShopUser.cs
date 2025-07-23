using Sellence.Shared.Models.Enums;

namespace Sellence.Data.Models.Users;

public sealed class ShopUser : User
{
    public ShopUser()
    {
        this.Discriminator = UserType.ShopUser;
    }
}
