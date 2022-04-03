using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace FinFolio.Core.Entities
{
    public partial class User : IdentityUser<int>
    {
        public User()
        {
            UserRoles = new HashSet<UserRole>();
        }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}