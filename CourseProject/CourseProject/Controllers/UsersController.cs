using CourseProject.Data;
using CourseProject.DataModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CourseProject.Controllers
{
    public class UsersController : Controller
    {
        private readonly EmploymentServiceContext _context;

        public UsersController(EmploymentServiceContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,RoleId,Username,Password,Name,Surname,Patronymic")] User user)
        {
            if (!UserExists(user.Username))
            {
                user.RoleId = 1;
                _context.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login", "Auth");
            }
            else
            {
                ModelState.AddModelError("", "Пользователь с таким именем уже существует");
                return View();
            }
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _context.Users.Include(u => u.Requests).ThenInclude(r => r.Event).FirstOrDefaultAsync(u => u.UserId == id);
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
        public async Task<IActionResult> Edit(int id, [Bind("UserId,RoleId,Username,Password,Name,Surname,Patronymic")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();

                var identity = (ClaimsIdentity)User.Identity;
                var existingClaim = identity.FindFirst("UserFullname");
                if (existingClaim != null) identity.RemoveClaim(existingClaim);
                identity.AddClaim(new Claim("UserFullname", $"{user.Surname} {user.Name} {user.Patronymic}"));

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return View(_context.Users.Include(u => u.Requests).ThenInclude(r => r.Event).FirstOrDefault());
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        private bool UserExists(string name)
        {
            return _context.Users.Any(e => e.Username == name);
        }
    }
}
