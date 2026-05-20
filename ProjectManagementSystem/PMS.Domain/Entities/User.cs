using Microsoft.AspNetCore.Identity;

namespace PMS.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public User()
    {
    }

}
