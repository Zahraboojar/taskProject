using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskProject.Models;
using TaskProject.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskProject.Services
{
    public class UserService : IUserService
    {
        private readonly TaskDbContext _context;

        public UserService(TaskDbContext context)
        {
            _context = context;
        }
        public async Task<int> DeleteUser(int id)
        {
            return await _context.Database.ExecuteSqlInterpolatedAsync(
                     $"EXEC dbo.DeleteUser {id}");
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users
                .FromSqlInterpolated($"EXEC dbo.SelectUsers")
                .ToListAsync();
        }

        public async Task<List<UserViewModel>> GetAllWithTasks()
        {
            var data = await _context.UserWithTaskListDto
                .FromSqlInterpolated($"EXEC dbo.SelectUsersWithTaskList")
                .ToListAsync();

            var vmList = data
     .GroupBy(x => new
     {
         x.Id,
         x.PhoneNumber,
         x.FullName,
         x.Username,
         x.Email,
         x.NationalCode,
     })
     .Select(g => new UserViewModel
     {
         Id = g.Key.Id,
         FullName = g.Key.FullName,
         PhoneNumber = g.Key.PhoneNumber,
         Email = g.Key.Email,
         Username = g.Key.Username,
         NationalCode = g.Key.NationalCode,

         Tasks = g
             .Where(x => x.TaskId != null)
             .GroupBy(x => x.TaskId)
             .Select(x => new SelectListItem
             {
                 Value = x.Key!.Value.ToString(),
                 Text = x.First().TaskTitle!
             })
             .ToList(),

     })
     .ToList();

            return vmList;
        }

        public List<SelectListItem> GetAllSelcted(List<User> Users)
        {
            return Users.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.FullName} ( {c.NationalCode} )"
            }).ToList();
        }

        public async Task<User> GetUser(int? id)
        {
            return (await _context.Users
                .FromSqlInterpolated($"EXEC dbo.SelectUserWithId {id}").ToListAsync())
                .SingleOrDefault();
        }
        public async Task<User> GetUser(string? username)
        {
            return (await _context.Users
               .FromSqlInterpolated($"EXEC dbo.SelectUserWithUsername {username}")
               .ToListAsync())
                .SingleOrDefault();
        }

        public async Task<int> InsertUser(User user)
        {
           return await _context.Database.ExecuteSqlInterpolatedAsync(
                             $"EXEC dbo.InsertUser {user.Username}, {user.FullName}, {user.NationalCode}, {user.Email}, {user.PhoneNumber}, {user.PasswordHash}");
        }

        public async Task<int> UpdateUser(int id, User user)
        {
           return await _context.Database.ExecuteSqlInterpolatedAsync(
              $"EXEC dbo.UpdateUser {user.Username}, {user.FullName}, {user.NationalCode}, {user.Email}, {user.PhoneNumber}, {user.PasswordHash}, {user.Id}");
        }
    }
}
