using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace FinFolio.Core.Entities
{
    public partial class Role : IdentityRole<int>
    {
        public Role()
        {
            this.UserRoles = new HashSet<UserRole>();
        }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
