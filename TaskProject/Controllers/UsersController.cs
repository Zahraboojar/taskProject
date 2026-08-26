using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaskProject.Models;
using TaskProject.Services;
using TaskProject.ViewModels;

namespace TaskProject.Controllers
{
    public class UsersController : Controller
    {
        private readonly TaskDbContext _context;
        private readonly IUserService _userService;
        private readonly ITaskUserService _taskUserService;

        public UsersController(TaskDbContext context, IUserService userService, ITaskUserService taskUserService)
        {
            _context = context;
            _userService = userService;
            _taskUserService = taskUserService;
        }

        // GET: Users
        public async Task<IActionResult> Index(UserFilterViewModel ufv)
        {
            //return View(await _context.Users.ToListAsync());
            var data = await _userService.GetAllWithTasks();

            
            if (!string.IsNullOrWhiteSpace(ufv.FullName))
            {
                data = data
                    .Where(x => x.FullName != null &&
                                x.FullName.Contains(ufv.FullName,
                                    StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            if (!string.IsNullOrWhiteSpace(ufv.Email))
            {
                data = data
                    .Where(x => x.Email != null &&
                                x.Email.Contains(ufv.Email,
                                    StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            if (!string.IsNullOrWhiteSpace(ufv.PhoneNumber))
            {
                data = data
                    .Where(x => x.PhoneNumber != null &&
                                x.PhoneNumber.Contains(ufv.PhoneNumber,
                                    StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            if (!string.IsNullOrWhiteSpace(ufv.NationalCode))
            {
                data = data
                    .Where(x => x.NationalCode != null &&
                                x.NationalCode.Contains(ufv.NationalCode,
                                    StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            if (!string.IsNullOrWhiteSpace(ufv.Username))
            {
                data = data
                    .Where(x => x.Username != null &&
                                x.Username.Contains(ufv.Username,
                                    StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Sort
            data = SortList(data, ufv);

            // Pagination
            data = data
                .Skip(ufv.Page * ufv.ItemCount)
                .Take(ufv.ItemCount)
                .ToList();

            // Total pages
            ufv.TotalPages = (int)Math.Ceiling(
                data.Count / (double)ufv.ItemCount
            );

            ViewBag.Filter = ufv;

            return View(data);
        }

        [NonAction]
        public List<UserViewModel> SortList(
   List<UserViewModel> list,
   UserFilterViewModel filter)
        {
            return filter.SortColumn switch
            {
                "FullName" => filter.SortDescending
                    ? list.OrderByDescending(x => x.FullName).ToList()
                    : list.OrderBy(x => x.FullName).ToList(),

                "Username" => filter.SortDescending
                    ? list.OrderByDescending(x => x.Username).ToList()
                    : list.OrderBy(x => x.Username).ToList(),

                "NationalCode" => filter.SortDescending
                    ? list.OrderByDescending(x => x.NationalCode).ToList()
                    : list.OrderBy(x => x.NationalCode).ToList(),

                "Email" => filter.SortDescending
                    ? list.OrderByDescending(x => x.Email).ToList()
                    : list.OrderBy(x => x.Email).ToList(),
                "PhoneNumber" => filter.SortDescending
               ? list.OrderByDescending(x => x.PhoneNumber).ToList()
               : list.OrderBy(x => x.PhoneNumber).ToList(),

                "Id" => filter.SortDescending
                    ? list.OrderByDescending(x => x.Id).ToList()
                    : list.OrderBy(x => x.Id).ToList(),

                _ => list.OrderBy(x => x.Id).ToList()
            };
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,NationalCode,PhoneNumber,Email,Username,PasswordHash")] User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!await UserExists(user.Username))
                    {
                         user.PasswordHash = GetMd5Hash(user.PasswordHash);

                        await _userService.InsertUser(user);

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError(
                        "Username",
                        "این نام کاربری قبلاً ثبت شده است."
                    );
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(
                        "Username",
                        "این نام کاربری قبلاً ثبت شده است."
                    );
                }
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,NationalCode,PhoneNumber,Email,Username,PasswordHash")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }
            var oldUser = await _userService.GetUser(id);

            if (ModelState.IsValid)
            {
                try
                {
                    if (!await UserExists(user.Username) || user.Username == oldUser?.Username)
                    {
                        user.PasswordHash = GetMd5Hash(user.PasswordHash);
                        await _userService.UpdateUser(id, user);
                        return RedirectToAction(nameof(Index));
                    } else
                    {
                        ModelState.AddModelError(
                        "Username",
                        "این نام کاربری قبلاً ثبت شده است."
                    );
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userService.GetUser(id);
            if (user != null)
            {
                await _userService.DeleteUser(id);
                await _taskUserService.DeleteTaskUserByUserId(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> UserExists(int id)
        {
            var user = await _userService.GetUser(id);
            if (user == null)
                return false;
            return true;
        }
        private async Task<bool> UserExists(string username)
        {
            var user = await _userService.GetUser(username);
            if (user == null)
                return false;
            return true;
        }
        public static string GetMd5Hash(string input)
        {
            using var md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (byte b in hashBytes)
                sb.Append(b.ToString("x2"));  // خروجی هگزادسیمال ۳۲ کاراکتری

            return sb.ToString();
        }
    }
}
