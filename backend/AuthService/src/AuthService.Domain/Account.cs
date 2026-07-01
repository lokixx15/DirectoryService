using Microsoft.AspNetCore.Identity;

namespace AuthService.Domain;

public class Account : IdentityUser<Guid>
{
    // ef core
    private Account() { }

    public Account(string email, string userName)
    {
        Id = Guid.CreateVersion7();
        Email = email;
        UserName = userName;
    }
}
