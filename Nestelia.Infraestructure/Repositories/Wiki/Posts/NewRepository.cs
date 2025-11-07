using Nestelia.Domain.Entities.Wiki.Posts;
using Nestelia.Infraestructure.Common;
using Nestelia.Infraestructure.Interfaces.Wiki.Posts;
using Nestelia.Infraestructure.Repositories.Generic;
using System.Security.Claims;

namespace Nestelia.Infraestructure.Repositories.Wiki.Posts
{
    public class NewRepository(ApplicationDbContext context, ClaimsPrincipal user) : BaseRepository<New>(context, user), INewRepository
    {
        private readonly ApplicationDbContext _context = context;
    }
}
