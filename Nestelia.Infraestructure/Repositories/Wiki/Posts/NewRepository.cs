using Microsoft.EntityFrameworkCore;
using Nestelia.Domain.Common.Util;
using Nestelia.Domain.Common.ViewModels.News;
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

        public async Task<PagedResult<NewsListVM>> GetNewsAsync(string param, int page, int pageSize)
        {
            var query = _context.News.AsQueryable();

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
                .OrderByDescending(n => n.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NewsListVM
                {
                    Id = n.Id,
                    Title = n.Title,
                    Description = n.Description,
                    PublishedAt = n.PublishedAt,
                    Content = n.Content,
                    AuthorName = n.Author != null ? n.Author.UserName! : "Autor desconocido",
                    CoverImageUrl = n.CoverImageUrl
                })
                
                .ToListAsync();

            return new PagedResult<NewsListVM>
            {
                Items = data,
                Page = page,
                Size = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<NewVM> GetNewByid(Guid id)
        {
            var news = await _context.News
                .Include(n => n.Author)
                .FirstOrDefaultAsync(n => n.Id == id);
            if (news == null)
            {
                return null!;
            }
            return new NewVM
            {
                Id = news.Id.ToString(),
                Title = news.Title,
                Description = news.Description,
                PublishedAt = news.PublishedAt,
                Content = news.Content,
                AuthorName = news.Author != null ? news.Author.UserName! : "Autor desconocido",
                CoverImageUrl = news.CoverImageUrl
            };

        }

    }
}
