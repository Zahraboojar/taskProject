using TaskProject.Models;

namespace TaskProject.Services
{
    public interface ICategoryService
    {
        public Task<int> InsertCategory(Category category);
        public Task<int> UpdateCategory(int id, Category category);
        public Task<int> DeleteCategory(int id);
    }
}
