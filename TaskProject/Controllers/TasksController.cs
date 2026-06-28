using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskProject.Models;
using TaskProject.ViewModels;

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
        public async Task<IActionResult> Index()
        {
            var tasks = await _context.Tasks
       .Include(t => t.CategoryTasks)
       .ThenInclude(ct => ct.Category)
       .ToListAsync();

            return View(tasks);
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
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

            return View(vm);
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = _context.Categories
                    .Select(c => new SelectListItem
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
                Status = vm.Status
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            foreach (var id in vm.SelectedCategoryIds)
            {
                if (id != 0)
                {
                    _context.CategoryTasks.Add(new CategoryTask
                    {
                        TaskId = task.Id,
                        CategoryId = id
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
                return NotFound();

            var vm = new TaskViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,

                Categories = _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Title
                    }).ToList(),

                SelectedCategoryIds = _context.CategoryTasks
                    .Where(ct => ct.TaskId == task.Id)
                    .Select(ct => ct.CategoryId)
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
            var task = await _context.Tasks.FindAsync(vm.Id);

            if (task == null)
                return NotFound();

            task.Title = vm.Title;
            task.Description = vm.Description;
            task.Status = vm.Status;

            var old = _context.CategoryTasks
                .Where(x => x.TaskId == task.Id);

            _context.CategoryTasks.RemoveRange(old);

            foreach (var id in vm.SelectedCategoryIds)
            {
                _context.CategoryTasks.Add(new CategoryTask
                {
                    TaskId = task.Id,
                    CategoryId = id
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
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
                _context.Tasks.Remove(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
