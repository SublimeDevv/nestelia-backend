using Microsoft.AspNetCore.Identity;

namespace Nestelia.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsDeleted { get; set; }
    }
}
