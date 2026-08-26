using Microsoft.AspNetCore.Mvc.Rendering;
using TaskProject.Models;
using TaskProject.ViewModels;

namespace TaskProject.Services
{
    public interface IUserService
    {
        public Task<int> InsertUser(User user);
        public Task<User> GetUser(int? id);
        public Task<User> GetUser(string? username);
        public Task<List<User>> GetAll();
        public Task<List<UserViewModel>> GetAllWithTasks();
        public List<SelectListItem> GetAllSelcted(List<User> Users);
        public Task<int> UpdateUser(int id, User user);
        public Task<int> DeleteUser(int id);
    }
}
