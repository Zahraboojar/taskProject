using TaskProject.Models;

namespace TaskProject.Services
{
    public interface ITaskUserService
    {
        public Task<int> InsertTaskUser(int taskId, int userId);
        public Task<int> DeleteTaskUserByTaskId(int taskId);
        public Task<int> DeleteTaskUserByUserId(int userId);
    }
}
