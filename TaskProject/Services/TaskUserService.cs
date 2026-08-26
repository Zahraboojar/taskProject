
using Microsoft.EntityFrameworkCore;
using TaskProject.Models;

namespace TaskProject.Services
{
    public class TaskUserService : ITaskUserService
    {
        private readonly TaskDbContext _context;

        public TaskUserService(TaskDbContext context)
        {
            _context = context;
        }

        public Task<int> DeleteTaskUserByTaskId(int taskId)
        {
            return _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.DeleteUserByTaskIDTask {taskId}");
        }

        public Task<int> DeleteTaskUserByUserId(int userId)
        {
            return _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.DeleteUserByUserIDTask {userId}");
        }

        public Task<int> InsertTaskUser(int taskId, int userId)
        {
            return _context.Database.ExecuteSqlInterpolatedAsync(
                       $"EXEC dbo.InsertTaskUser {userId}, {taskId}");
        }
    }
}
