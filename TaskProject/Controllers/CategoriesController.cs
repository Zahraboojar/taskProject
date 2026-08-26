using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskProject.Models;
using TaskProject.Services;
using TaskProject.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskProject.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly TaskDbContext _context;
        private readonly ICategoryService _categoryService;
        private readonly ICategoryTaskService _categoryTaskService;

        public CategoriesController(TaskDbContext context, ICategoryService categoryService, ICategoryTaskService categoryTaskService)
        {
            _context = context;
            _categoryService = categoryService;
            _categoryTaskService = categoryTaskService;

        }

        // GET: Categories
        public async Task<IActionResult> Index(CategoryFilterViewModel cfv)
        {
            var categories = await _categoryService.GetAll();

            if (!string.IsNullOrWhiteSpace(cfv.Title))
            {
                categories = categories
                    .Where(x => x.Title != null &&
                                x.Title.Contains(cfv.Title,
                                    StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Sort
            categories = SortList(categories, cfv);

            // Pagination
            categories = categories 
                .Skip(cfv.Page * cfv.ItemCount)
                .Take(cfv.ItemCount)
                .ToList();

            // Total pages
            cfv.TotalPages = (int)Math.Ceiling(
                categories.Count / (double)cfv.ItemCount
            );

            ViewBag.Filter = cfv;

            return View(categories);
        }

        public List<Category> SortList(
   List<Category> list,
   CategoryFilterViewModel filter)
        {
            return filter.SortColumn switch
            {
                "Title" => filter.SortDescending
                    ? list.OrderByDescending(x => x.Title).ToList()
                    : list.OrderBy(x => x.Title).ToList(),

                "Description" => filter.SortDescending
                    ? list.OrderByDescending(x => x.Description).ToList()
                    : list.OrderBy(x => x.Description).ToList(),

                "Id" => filter.SortDescending
                    ? list.OrderByDescending(x => x.Id).ToList()
                    : list.OrderBy(x => x.Id).ToList(),

                _ => list.OrderBy(x => x.Id).ToList()
            };
        }
        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.InsertCategory(category);
                //_context.Add(category);
                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _categoryService.UpdateCategory(id, category);
                    //_context.Update(category);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }

            var count = await _categoryTaskService.CountCategoryTask(id);

            ViewData["IsShowConfirmAlert"] = false;
            if (count > 0)
            {
                ViewData["IsShowConfirmAlert"] = true;
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                await _categoryService.DeleteCategory(id);
                await _categoryTaskService.DeleteCategoryTaskByCategoryId(id);
                //_context.Categories.Remove(category);
            }

            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
