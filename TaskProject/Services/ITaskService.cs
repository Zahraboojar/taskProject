namespace TaskProject.Services
{
    public interface ITaskService
    {
        public Task<int> InsertTask(Task task);
        public Task<int> UpdateTask(int id, Task task);
        public Task<int> DeleteTask(int id);
    }
}
