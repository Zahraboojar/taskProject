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
        public Task<int> DeleteCategory(int id)
        {
            throw new NotImplementedException();
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

        public Task<Category> GetCategory(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateCategory(int id, Category category)
        {
            throw new NotImplementedException();
        }
    }
}
