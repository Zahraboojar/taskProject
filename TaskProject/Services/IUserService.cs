using Microsoft.AspNetCore.Mvc.Rendering;
using TaskProject.Models;

namespace TaskProject.Services
{
    public interface IUserService
    {
        public Task<int> InsertUser(User user);
        public Task<User> GetUser(int? id);
        public Task<List<User>> GetAll();
        public List<SelectListItem> GetAllSelcted(List<User> Users);
        public Task<int> UpdateUser(int id, User user);
        public Task<int> DeleteUser(int id);
    }
}
