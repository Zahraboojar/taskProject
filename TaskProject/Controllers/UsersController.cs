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
using TaskProject.ViewModels;

namespace TaskProject.Controllers
{
    public class UsersController : Controller
    {
        private readonly TaskDbContext _context;

        public UsersController(TaskDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Users.ToListAsync());
            var data = await _context.UserWithTaskListDto
                .FromSqlInterpolated($"EXEC dbo.SelectUsersWithTaskList")
                .ToListAsync();

            var vmList = data
     .GroupBy(x => new
     {
         x.Id,
         x.PhoneNumber,
         x.FullName,
         x.Username,
         x.Email,
         x.NationalCode,
     })
     .Select(g => new UserViewModel
     {
         Id = g.Key.Id,
         FullName = g.Key.FullName,
         PhoneNumber = g.Key.PhoneNumber,
         Email = g.Key.Email,
         Username = g.Key.Username,
         NationalCode = g.Key.NationalCode,

         Tasks = g
             .Where(x => x.TaskId != null)
             .GroupBy(x => x.TaskId)
             .Select(x => new SelectListItem
             {
                 Value = x.Key!.Value.ToString(),
                 Text = x.First().TaskTitle!
             })
             .ToList(),

     })
     .ToList();

            return View(vmList);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = (await _context.Users
                .FromSqlInterpolated($"EXEC dbo.SelectUserWithId {id}").ToListAsync())
                .SingleOrDefault();
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
                        var finalPass = GetMd5Hash(user.PasswordHash);
                        await _context.Database.ExecuteSqlInterpolatedAsync(
                              $"EXEC dbo.InsertUser {user.Username}, {user.FullName}, {user.NationalCode}, {user.Email}, {user.PhoneNumber}, {finalPass}");
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

            var user = (await _context.Users
                .FromSqlInterpolated($"EXEC dbo.SelectUserWithId {id}").ToListAsync())
                .SingleOrDefault();
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
            var oldUser = (await _context.Users
                .FromSqlInterpolated($"EXEC dbo.SelectUserWithId {id}").ToListAsync())
                .SingleOrDefault();

            if (ModelState.IsValid)
            {
                try
                {
                    if (!await UserExists(user.Username) || user.Username == oldUser?.Username)
                    {
                        var finalPass = GetMd5Hash(user.PasswordHash);
                        await _context.Database.ExecuteSqlInterpolatedAsync(
                          $"EXEC dbo.UpdateUser {user.Username}, {user.FullName}, {user.NationalCode}, {user.Email}, {user.PhoneNumber}, {finalPass}, {user.Id}");
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

            var user = (await _context.Users
                .FromSqlInterpolated($"EXEC dbo.SelectUserWithId {id}").ToListAsync())
                .SingleOrDefault();
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
            var user = (await _context.Users
                .FromSqlInterpolated($"EXEC dbo.SelectUserWithId {id}").ToListAsync())
                .SingleOrDefault();
            if (user != null)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                      $"EXEC dbo.DeleteUser {id}");
                await _context.Database.ExecuteSqlInterpolatedAsync(
                     $"EXEC dbo.DeleteTaskUserByUserID {id}");
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> UserExists(int id)
        {
            var user = (await _context.Users
               .FromSqlInterpolated($"EXEC dbo.SelectUserWithId {id}")
               .ToListAsync())
                .SingleOrDefault();
            if (user == null)
                return false;
            return true;
        }
        private async Task<bool> UserExists(string username)
        {
            var user = (await _context.Users
               .FromSqlInterpolated($"EXEC dbo.SelectUserWithUsername {username}")
               .ToListAsync())
                .SingleOrDefault();
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
