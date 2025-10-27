using Nestelia.Domain.Entities.Wiki.Categories;
using Nestelia.Infraestructure.Common;
using Nestelia.Infraestructure.Interfaces.Wiki.Categories;
using Nestelia.Infraestructure.Repositories.Generic;
using System.Security.Claims;

namespace Nestelia.Infraestructure.Repositories.Wiki.Categories
{
    public class CategoryRepository(ApplicationDbContext context, ClaimsPrincipal user) : BaseRepository<Category>(context, user), ICategoryRepository
    {
        private readonly ApplicationDbContext _context = context;
    }
}
