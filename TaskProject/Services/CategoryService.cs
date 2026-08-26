using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskProject.Models;

namespace TaskProject.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly TaskDbContext _context;

        public CategoryService(TaskDbContext context)
        {
            _context = context;
        }
        public async Task<int> DeleteCategory(int id)
        {
            return await _context.Database.ExecuteSqlInterpolatedAsync(
                         $"EXEC dbo.DeleteCategory {id}");
        }

        public async Task<List<Category>> GetAll()
        {
            return await _context.Categories
                .FromSqlInterpolated($"EXEC dbo.SelectCategories")
                .ToListAsync();
        }

        public List<SelectListItem> GetAllSelcted(List<Category> categories)
        {
           return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Title
            }).ToList();
        }

        public async Task<Category> GetCategory(int? id)
        {
            return (await _context.Categories
    .FromSqlInterpolated($"EXEC dbo.CategoryDetail {id}")
    .ToListAsync())
    .SingleOrDefault();
        }

        public async Task<int> InsertCategory(Category category)
        {
            return await _context.Database.ExecuteSqlInterpolatedAsync(
                      $"EXEC dbo.InsertCategory {category.Title}, {category.Description}");
        }

        public async Task<int> UpdateCategory(int id, Category category)
        {
           return await _context.Database.ExecuteSqlInterpolatedAsync(
                         $"EXEC dbo.UpdateCategory {category.Title}, {category.Description}, {id}");
        }
    }
}
