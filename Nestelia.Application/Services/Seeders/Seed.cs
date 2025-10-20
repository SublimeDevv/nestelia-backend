using Microsoft.AspNetCore.Identity;
using Nestelia.Domain.Entities;
using Nestelia.Infraestructure.Common;

namespace Nestelia.Application.Services.Seeders
{
    public class Seed(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    }
}
