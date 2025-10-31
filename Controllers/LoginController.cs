using Microsoft.AspNetCore.Mvc;
using HRMS_CSharp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace HRMS_CSharp.Controllers
{
    public class LoginController : Controller
    {
        private readonly HrmsContext _context;

        public LoginController(HrmsContext context)
        {
            _context = context;
        }

        // GET: Login/Index
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("user_login_access") == "1")
                return RedirectToAction("Index", "Dashboard");

            return View("Login");
        }

        // POST: Login/Login_Auth
        [HttpPost]
        public async Task<IActionResult> Login_Auth(string email, string password, bool remember = false)
        {
            // Basic validation
            if (string.IsNullOrEmpty(email) || email.Length < 7)
            {
                TempData["feedback"] = "UserEmail or Password is Invalid";
                return RedirectToAction("Index");
            }
            if (string.IsNullOrEmpty(password) || password.Length < 6)
            {
                TempData["feedback"] = "UserEmail or Password is Invalid";
                return RedirectToAction("Index");
            }

            string hashedPassword = HashPassword(password);

            var user = await _context.Employees.FirstOrDefaultAsync(e =>
                e.EmEmail == email &&
                e.EmPassword == hashedPassword &&
                e.EmStatus == "ACTIVE");

            if (user != null)
            {
                HttpContext.Session.SetString("user_login_access", "1");
                HttpContext.Session.SetString("user_login_id", user.EmId ?? "");
                HttpContext.Session.SetString("name", user.FirstName ?? "");
                HttpContext.Session.SetString("email", user.EmEmail ?? "");
                HttpContext.Session.SetString("user_image", user.EmImage ?? "");
                HttpContext.Session.SetString("user_type", user.EmRole ?? "");

                if (remember)
                {
                    Response.Cookies.Append("email", email, new CookieOptions { Expires = DateTime.Now.AddDays(30) });
                    Response.Cookies.Append("password", password, new CookieOptions { Expires = DateTime.Now.AddDays(30) });
                }
                else
                {
                    Response.Cookies.Delete("email");
                    Response.Cookies.Delete("password");
                }

                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                TempData["feedback"] = "UserEmail or Password is Invalid";
                return RedirectToAction("Index");
            }
        }

        // GET: Login/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["feedback"] = "logged_out";
            return Redirect("/");
        }

        private string HashPassword(string password)
        {
            using (var sha1 = SHA1.Create())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
