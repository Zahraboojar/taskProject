
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskProject.Models;
using TaskProject.Services;
using TaskProject.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskProject.Controllers
{
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly TaskDbContext _context;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;

        public TasksController(
    TaskDbContext context,
    ITaskService taskService,
    ICategoryService categoryService,
    IUserService userService)
        {
            _context = context;
            _taskService = taskService;
            _categoryService = categoryService;
            _userService = userService;
        }

        public async Task<IActionResult> Index(TaskFilterViewModel tfv)
        {
            var dataList = await _taskService.GetAll();

            // Filter - Title
            if (!string.IsNullOrWhiteSpace(tfv.Title))
            {
                dataList = dataList
                    .Where(x => x.Title != null &&
                                x.Title.Contains(tfv.Title,
                                    StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter - Status
            if (tfv.Status.HasValue)
            {
                dataList = dataList
                    .Where(x => x.Status == tfv.Status.Value)
                    .ToList();
            }

            // Filter - DueDate
            if (tfv.DueDate.HasValue)
            {
                if (tfv.DueDateSearchType == 1)
                {
                    dataList = dataList
                        .Where(x => x.DueDate == tfv.DueDate)
                        .ToList();
                }
                else if (tfv.DueDateSearchType == 2)
                {
                    dataList = dataList
                        .Where(x => x.DueDate >= tfv.DueDate)
                        .ToList();
                }
                else if (tfv.DueDateSearchType == 3)
                {
                    dataList = dataList
                        .Where(x => x.DueDate <= tfv.DueDate)
                        .ToList();
                }
            }

            // Sort
            dataList = SortList(dataList, tfv);

            // Pagination
            dataList = dataList
                .Skip(tfv.Page * tfv.ItemCount)
                .Take(tfv.ItemCount)
                .ToList();

            // Total pages
            tfv.TotalPages = (int)Math.Ceiling(
                dataList.Count / (double)tfv.ItemCount
            );

            ViewBag.Filter = tfv;

            return View(dataList);
        }
        [NonAction]
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

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _taskService.GetTask(id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public async Task<IActionResult> Create()
        {
            var vm = new TaskViewModel();

            var categories = await _categoryService.GetAll();
            vm.Categories = _categoryService.GetAllSelcted(categories);

            var users = await _userService.GetAll();
            vm.Users = _userService.GetAllSelcted(users);

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
                var categories = await _categoryService.GetAll();
                vm.Categories = _categoryService.GetAllSelcted(categories);

                var users = await _userService.GetAll();
                vm.Users = _userService.GetAllSelcted(users);

                return View(vm);
            }

            await _taskService.InsertTask(vm);

            return RedirectToAction(nameof(Index));
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var task = await _taskService.GetTask(id);

            if (task == null)
                return NotFound();
            var categories = await _categoryService.GetAll();

            var users = await _userService.GetAll();

            var vm = new TaskViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,

                Categories =  _categoryService.GetAllSelcted(categories) ,
                Users = _userService.GetAllSelcted(users),

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
            var task = await _taskService.GetTask(vm.Id);

            if (task == null)
                return NotFound();

            await _taskService.UpdateTask(vm.Id, vm);

            return RedirectToAction(nameof(Index));
        }


        // GET: Tasks/ChangeStatus/5
        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null)
                return NotFound();

            var task = await _taskService.GetTask(id);

            if (task == null)
                return NotFound();

            return View(task);
        }

        // POST: Tasks/ChangeStatus/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(Models.Task data)
        {
            var task = await _taskService.GetTask(data.Id);

            if (task == null)
                return NotFound();

            await _taskService.ChangeStatusTask(data.Id, data.Status);

            return RedirectToAction(nameof(Index));
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _taskService.GetTask(id);
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
            var task = await _taskService.GetTask(id);
            if (task != null)
            {
                await _taskService.DeleteTask(id);
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TaskExists(int id)
        {
            var data = await _taskService.GetTask(id);
            if (data == null)
                return false;
            return true;
        }
    }
}
