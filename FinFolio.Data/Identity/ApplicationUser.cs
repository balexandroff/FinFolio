using Microsoft.AspNetCore.Identity;

namespace FinFolio.Data.Identity
{
    public class ApplicationUser : IdentityUser<string> { }
    public class ApplicationRole : IdentityRole<string> { }
    public class ApplicationRoleClaim : IdentityRoleClaim<string> { }
    public class ApplicationUserClaim : IdentityUserClaim<string> { }
    public class ApplicationUserLogin : IdentityUserLogin<string> { }
    public class ApplicationUserToken : IdentityUserToken<string> { }
    public class ApplicationUserRole : IdentityUserRole<string> { }
}
