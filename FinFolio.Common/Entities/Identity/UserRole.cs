using Microsoft.AspNetCore.Identity;

namespace FinFolio.Core.Entities
{
    public partial class UserRole : IdentityUserRole<int>
    {
        public virtual Role Role { get; set; }

        public virtual User User { get; set; }
    }
}