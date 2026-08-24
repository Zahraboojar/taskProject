using TaskProject.Models;

namespace TaskProject.Services
{
    public interface IUserService
    {
        public Task<int> InsertUser(User user);
        public Task<int> UpdateUser(int id, User user);
        public Task<int> DeleteUser(int id);
    }
}
