
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
        public Task<int> DeleteCategoryTaskByCategoryId(int categoryId)
        {
            return _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.DeleteCategoryTaskByCategoryID {categoryId}");
        }

        public Task<int> DeleteCategoryTaskByTaskId(int taskId)
        {
            return _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.DeleteCategoryTaskByTaskIDTask {taskId}");
        }

        public Task<int> InsertCategoryTask(int taskId, int categoryId)
        {
            return _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.InsertCategoryTask {categoryId},{taskId}");
        }
    }
}
