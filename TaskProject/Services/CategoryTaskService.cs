
using Microsoft.EntityFrameworkCore;
using TaskProject.Models;

namespace TaskProject.Services
{
    public class CategoryTaskService : ICategoryTaskService
    {
        private readonly TaskDbContext _context;

        public CategoryTaskService(TaskDbContext context)
        {
            _context = context;
        }
        public async Task<int> DeleteCategoryTaskByCategoryId(int categoryId)
        {
            return await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.DeleteCategoryTaskByCategoryID {categoryId}");
        }

        public async Task<int> DeleteCategoryTaskByTaskId(int taskId)
        {
            return await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.DeleteCategoryTaskByTaskID {taskId}");
        }

        public async Task<int> InsertCategoryTask(int taskId, int categoryId)
        {
            return await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.InsertCategoryTask {categoryId},{taskId}");
        }
        public async Task<int> CountCategoryTask(int? categoryId)
        {
            return (await _context.Categories
    .FromSqlInterpolated($"EXEC dbo.SelectCategoryTask {categoryId}")
    .ToListAsync()).Count;
        }
    }
}
