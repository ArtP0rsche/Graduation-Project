using CourseProject.Data;
using CourseProject.DataModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourseProject.Controllers
{
    public class AuthController : Controller
    {
        private readonly EmploymentServiceContext _context;

        public AuthController(EmploymentServiceContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([Bind("Username,Password")] User @user)
        {
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == @user.Username);

            if (currentUser != null && currentUser.Password == user.Password)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, @user.Username),
                    new Claim("RoleId", currentUser.RoleId.ToString(), ClaimValueTypes.Integer32),
                    new Claim("UserID", currentUser.UserId.ToString(), ClaimValueTypes.Integer32),
                    new Claim("UserFullname", $"{currentUser.Surname} {currentUser.Name} {currentUser.Patronymic}", ClaimValueTypes.String)
                };
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);

                return RedirectToAction("Index", "Events");
            }
            else
            {
                TempData["AuthError"] = "Неверный логин или пароль";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Events");
        }
    }
}
