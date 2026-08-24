using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskProject.Models;

namespace TaskProject.Services
{
    public class UserService : IUserService
    {
        private readonly TaskDbContext _context;

        public UserService(TaskDbContext context)
        {
            _context = context;
        }
        public Task<int> DeleteUser(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users
                .FromSqlInterpolated($"EXEC dbo.SelectUsers")
                .ToListAsync();
        }

        public List<SelectListItem> GetAllSelcted(List<User> Users)
        {
            return Users.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.FullName} ( {c.NationalCode} )"
            }).ToList();
        }

        public Task<User> GetUser(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertUser(User user)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateUser(int id, User user)
        {
            throw new NotImplementedException();
        }
    }
}
