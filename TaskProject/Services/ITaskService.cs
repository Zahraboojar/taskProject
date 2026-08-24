using TaskProject.ViewModels;

namespace TaskProject.Services
{
    public interface ITaskService
    {
        public Task InsertTask(TaskViewModel task);
        public Task<int> ChangeStatusTask(int id, TasksStatus status);
        public Task<Models.Task> GetTask(int? id);
        public Task<List<TaskViewModel>> GetAll();
        public Task UpdateTask(int id, TaskViewModel task);
        public Task<int> DeleteTask(int id);
    }
}
