using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskProject.Models;
using TaskProject.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskProject.Services
{
    public class TaskService : ITaskService
    {
        private readonly TaskDbContext _context;

        public TaskService(TaskDbContext context)
        {
            _context = context;
        }

        public async Task<int> ChangeStatusTask(int id, TasksStatus status)
        {
            return await _context.Database.ExecuteSqlInterpolatedAsync(
                       $"EXEC dbo.ChangeTaskStatus {status}, {id}");
        }

        public async Task<int> DeleteTask(int id)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                       $"EXEC dbo.DeleteCategoryByTaskIDTask {id}");
            await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.DeleteUserByTaskIDTask {id}");
            return await _context.Database.ExecuteSqlInterpolatedAsync(
                         $"EXEC dbo.DeleteTask {id}");

        }

        public async Task<List<TaskViewModel>> GetAll()
        {
            var data = await _context.TaskDetailsDto
                .FromSqlInterpolated($"EXEC dbo.SelectTasksWithDetails")
                .ToListAsync();

            var vmList = data
     .GroupBy(x => new
     {
         x.Id,
         x.Title,
         x.Description,
         x.DueDate,
         x.Status
     })
     .Select(g => new TaskViewModel
     {
         Id = g.Key.Id,
         Title = g.Key.Title,
         DueDate = g.Key.DueDate,
         Description = g.Key.Description,
         Status = g.Key.Status,

         Categories = g
             .Where(x => x.CategoryId != null)
             .GroupBy(x => x.CategoryId)
             .Select(x => new SelectListItem
             {
                 Value = x.Key!.Value.ToString(),
                 Text = x.First().CategoryTitle!
             })
             .ToList(),

         Users = g
             .Where(x => x.UserId != null)
             .GroupBy(x => x.UserId)
             .Select(x => new SelectListItem
             {
                 Value = x.Key!.Value.ToString(),
                 Text = x.First().UserTitle!
             })
             .ToList()
     })
     .ToList();

            return vmList;
        }

        public async Task<Models.Task> GetTask(int? id)
        {
            var task = (await _context.Tasks
     .FromSqlInterpolated($"EXEC dbo.TaskDetail {id}")
     .ToListAsync())
     .SingleOrDefault();

            return task;
        }

        public async System.Threading.Tasks.Task InsertTask(TaskViewModel vm)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                         $"EXEC dbo.InsertTask {vm.Title}, {vm.Description}, {vm.DueDate}");
            
            var newTaskId = await _context.Tasks
    .OrderByDescending(t => t.Id)
    .Select(t => t.Id)
    .FirstOrDefaultAsync();

            foreach (var id in vm.SelectedCategoryIds)
            {
                if (id != 0)
                {
                    await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.InsertCategoryTask {id},{newTaskId}");
                }
            }
            foreach (var id in vm.SelectedUserIds)
            {
                if (id != 0)
                {
                    await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.InsertTaskUser {id},{newTaskId}");
                }
            }
        }

        public List<TaskViewModel> SortList(
    List<TaskViewModel> list,
    TaskFilterViewModel filter)
        {
            return filter.SortColumn switch
            {
                "Title" => filter.SortDescending
                    ? list.OrderByDescending(x => x.Title).ToList()
                    : list.OrderBy(x => x.Title).ToList(),

                "Description" => filter.SortDescending
                    ? list.OrderByDescending(x => x.Description).ToList()
                    : list.OrderBy(x => x.Description).ToList(),

                "DueDate" => filter.SortDescending
                    ? list.OrderByDescending(x => x.DueDate).ToList()
                    : list.OrderBy(x => x.DueDate).ToList(),

                "Status" => filter.SortDescending
                    ? list.OrderByDescending(x => x.Status).ToList()
                    : list.OrderBy(x => x.Status).ToList(),

                "Id" => filter.SortDescending
                    ? list.OrderByDescending(x => x.Id).ToList()
                    : list.OrderBy(x => x.Id).ToList(),

                _ => list.OrderBy(x => x.Id).ToList()
            };
        }

        public async System.Threading.Tasks.Task UpdateTask(int id, TaskViewModel vm)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                         $"EXEC dbo.DeleteCategoryByTaskIDTask {id}");
            await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.DeleteUserByTaskIDTask {id}");

            foreach (var cId in vm.SelectedCategoryIds)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.InsertCategoryTask {cId}, {id}");
                foreach (var uId in vm.SelectedUserIds)
                {
                    await _context.Database.ExecuteSqlInterpolatedAsync(
                            $"EXEC dbo.InsertTaskUser {uId}, {id}");
                }
                await _context.Database.ExecuteSqlInterpolatedAsync(
                          $"EXEC dbo.UpdateTask {vm.Title}, {vm.Description}, {vm.DueDate}, {id}");
            }
        }
    }
}
