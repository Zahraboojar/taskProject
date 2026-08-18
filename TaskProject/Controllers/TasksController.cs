using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskProject.Models;
using TaskProject.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskProject.Controllers
{
    public class TasksController : Controller
    {
        private readonly TaskDbContext _context;

        public TasksController(TaskDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index(TasksStatus? status)
        {
            ViewData["status"] = status;

            var data = await _context.TaskDetailsDto
                .FromSqlInterpolated($"EXEC dbo.SelectTasksWithDetails {status}")
                .ToListAsync();

            var vmList = data
     .GroupBy(x => new
     {
         x.Id,
         x.Title,
         x.Description,
         x.Status
     })
     .Select(g => new TaskViewModel
     {
         Id = g.Key.Id,
         Title = g.Key.Title,
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

            if (status != null)
            {
                vmList = vmList.Where(x => x.Status == status).ToList();
            }
            return View(vmList);
        }
        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = (await _context.Tasks
    .FromSqlInterpolated($"EXEC dbo.TaskDetail {id}")
    .ToListAsync())
    .SingleOrDefault();
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            var vm = new TaskViewModel();

            vm.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Title
                }).ToList();
            vm.Users = _context.Users
               .Select(c => new SelectListItem
               {
                   Value = c.Id.ToString(),
                   Text = $"{c.FullName} ( {c.NationalCode} )"
               }).ToList();

            return View(vm);
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskViewModel vm)
        {
            if (vm.SelectedCategoryIds == null)
            {
                vm.SelectedCategoryIds = new List<int>();
            }
            if (!ModelState.IsValid)
            {
                var categories = await _context.Categories
                .FromSqlInterpolated($"EXEC dbo.SelectCategories")
                .ToListAsync();
                vm.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Title
                }).ToList();

                return View(vm);
            }

            var task = new Models.Task
            {
                Title = vm.Title,
                Description = vm.Description,
                Status = TasksStatus.Pending,
            };
            await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.InsertTask {task.Title}, {task.Description}, {task.Status}");
            //_context.Tasks.Add(task);
            //await _context.SaveChangesAsync();

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
                    //_context.CategoryTasks.Add(new CategoryTask
                    //{
                    //    TaskId = task.Id,
                    //    CategoryId = id
                    //});
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

            //await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var task = (await _context.Tasks
.FromSqlInterpolated($"EXEC dbo.TaskDetail {id}")
.ToListAsync())
.SingleOrDefault();

            if (task == null)
                return NotFound();
            var categories = await _context.Categories
               .FromSqlInterpolated($"EXEC dbo.SelectCategories")
               .ToListAsync();
            var users = await _context.Users
               .FromSqlInterpolated($"EXEC dbo.SelectUsers")
               .ToListAsync();

            var vm = new TaskViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                //Status = task.Status,

                Categories = categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Title
                    }).ToList(),
                Users = users
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = $"{c.FullName} ( {c.NationalCode} )"
                    }).ToList(),

                SelectedCategoryIds = _context.CategoryTasks
                    .Where(ct => ct.TaskId == task.Id)
                    .Select(ct => ct.CategoryId)
                    .ToList()
            ,
                SelectedUserIds = _context.TaskUsers
                    .Where(ct => ct.TaskId == task.Id)
                    .Select(ct => ct.UserId)
                    .ToList()
            };


            return View(vm);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskViewModel vm)
        {
            var task = (await _context.Tasks
.FromSqlInterpolated($"EXEC dbo.TaskDetail {vm.Id}")
.ToListAsync())
.SingleOrDefault();

            if (task == null)
                return NotFound();

            task.Title = vm.Title;
            task.Description = vm.Description;
            task.Status = vm.Status;

            //var old = _context.CategoryTasks
            //    .Where(x => x.TaskId == task.Id);
            await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.DeleteCategoryByTaskIDTask {task.Id}");
            await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.DeleteUserByTaskIDTask {task.Id}");

            //_context.CategoryTasks.RemoveRange(old);

            foreach (var id in vm.SelectedCategoryIds)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.InsertCategoryTask {id}, {task.Id}");
                //_context.CategoryTasks.Add(new CategoryTask
                //{
                //    TaskId = task.Id,
                //    CategoryId = id
                //});
            }
            foreach (var id in vm.SelectedUserIds)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"EXEC dbo.InsertTaskUser {id}, {task.Id}");
                //_context.CategoryTasks.Add(new CategoryTask
                //{
                //    TaskId = task.Id,
                //    CategoryId = id
                //});
            }
            await _context.Database.ExecuteSqlInterpolatedAsync(
                      $"EXEC dbo.UpdateTask {task.Title}, {task.Description}, {task.Id}");
            //await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Tasks/ChangeStatus/5
        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null)
                return NotFound();

            var task = (await _context.Tasks
.FromSqlInterpolated($"EXEC dbo.TaskDetail {id}")
.ToListAsync())
.SingleOrDefault();

            if (task == null)
                return NotFound();

            return View(task);
        }

        // POST: Tasks/ChangeStatus/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(Models.Task data)
        {
            var task = (await _context.Tasks
.FromSqlInterpolated($"EXEC dbo.TaskDetail {data.Id}")
.ToListAsync())
.SingleOrDefault();

            if (task == null)
                return NotFound();

            task.Status = data.Status;

            await _context.Database.ExecuteSqlInterpolatedAsync(
                       $"EXEC dbo.ChangeTaskStatus {data.Status}, {task.Id}");

            return RedirectToAction(nameof(Index));
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = (await _context.Tasks
 .FromSqlInterpolated($"EXEC dbo.TaskDetail {id}")
 .ToListAsync())
 .SingleOrDefault();
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                         $"EXEC dbo.DeleteTask {id}");
                //_context.Tasks.Remove(task);
            }

            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
