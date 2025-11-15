using Dapper;
using Microsoft.EntityFrameworkCore;
using Nestelia.Domain.Common.ViewModels.Category;
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

        public async Task<List<CategoryListVM>> GetListCategories()
        {
            var sql = @"
                SELECT 
                    c.Id,
                    c.Name,
                    c.DisplayName,
                    c.Description,
                    c.Icon,
                    c.CreatedAt,
                    COUNT(a.Id) AS EntriesCount
                FROM 
                    Categories c
                LEFT JOIN 
                    WikiEntries a ON c.Id = a.CategoryId
                WHERE c.IsDeleted = 0
                GROUP BY 
                    c.Id, c.Name, c.DisplayName, c.Description, c.Icon, c.CreatedAt
                ORDER BY 
                    c.DisplayName;";

            var connection = _context.Database.GetDbConnection();
            var categories = await connection.QueryAsync<CategoryListVM>(sql);
            return [.. categories];

        }
    }

}
