using Microsoft.AspNetCore.Identity;
using Nestelia.Domain.Entities;
using Nestelia.Infraestructure.Common;

namespace Nestelia.Application.Services.Seeders
{
    public class Seed
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public Seed(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
    }
}
