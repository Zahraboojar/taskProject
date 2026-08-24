using Microsoft.AspNetCore.Mvc.Rendering;
using TaskProject.Models;

namespace TaskProject.Services
{
    public interface ICategoryService
    {
        public Task<int> InsertCategory(Category category);
        public Task<Category> GetCategory(int? id);
        public Task<List<Category>> GetAll();
        public List<SelectListItem> GetAllSelcted(List<Category> categories);
        public Task<int> UpdateCategory(int id, Category category);
        public Task<int> DeleteCategory(int id);
    }
}
