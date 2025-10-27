using Nestelia.Domain.Entities.Wiki.Posts;
using Nestelia.Infraestructure.Common;
using Nestelia.Infraestructure.Interfaces.Wiki.Posts;
using Nestelia.Infraestructure.Repositories.Generic;
using System.Security.Claims;

namespace Nestelia.Infraestructure.Repositories.Wiki.Posts
{
    public class CommentRepository(ApplicationDbContext context, ClaimsPrincipal user) : BaseRepository<Comment>(context, user), ICommentRepository
    {
        private readonly ApplicationDbContext _context = context;
    }
}
