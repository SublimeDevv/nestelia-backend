using Microsoft.EntityFrameworkCore;
using Nestelia.Domain.Common.Util;
using Nestelia.Domain.Common.ViewModels.News;
using Nestelia.Domain.Common.ViewModels.Posts;
using Nestelia.Domain.Entities.Wiki.Posts;
using Nestelia.Infraestructure.Common;
using Nestelia.Infraestructure.Interfaces.Wiki.Posts;
using Nestelia.Infraestructure.Repositories.Generic;
using System.Security.Claims;

namespace Nestelia.Infraestructure.Repositories.Wiki.Posts
{
    public class PostRepository(ApplicationDbContext context, ClaimsPrincipal user) : BaseRepository<Post>(context, user), IPostRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<PagedResult<PostsListVM>> GetPostsAsync(string param, int page, int pageSize)
        {
            var query = _context.Posts.AsQueryable();
            if (!string.IsNullOrWhiteSpace(param))
            {
                query = query.Where(n =>
                    n.Title.Contains(param) ||
                    n.Description.Contains(param) ||
                    n.Content.Contains(param)
                );
            }
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var data = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new PostsListVM
                {
                    Id = n.Id,
                    Title = n.Title,
                    Description = n.Description,
                    CreatedAt = n.CreatedAt,
                    Content = n.Content,
                    AuthorName = n.Author != null ? n.Author.UserName! : "Autor desconocido",
                    CoverImageUrl = n.CoverImageUrl
                })
                .ToListAsync();
            return new PagedResult<PostsListVM>
            {
                Items = data,
                Page = page,
                Size = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }


    }
}