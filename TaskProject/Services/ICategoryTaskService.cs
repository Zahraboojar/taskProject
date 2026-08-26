using TaskProject.Models;

namespace TaskProject.Services
{
    public interface ICategoryTaskService
    {
        public Task<int> InsertCategoryTask(int taskId, int categoryId);
        public Task<int> DeleteCategoryTaskByTaskId(int taskId);
        public Task<int> DeleteCategoryTaskByCategoryId(int categoryId);
        public Task<int> CountCategoryTask(int? categoryId);
    }
}
