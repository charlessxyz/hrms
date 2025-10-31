using Microsoft.AspNetCore.Mvc;
using HRMS_CSharp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace HRMS_CSharp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly HrmsContext _context;

        public DashboardController(HrmsContext context)
        {
            _context = context;
        }

        // GET: Dashboard/Index
        public IActionResult Index()
        {
            // Redirect to Admin dashboard after authentication
            if (HttpContext.Session.GetString("user_login_access") == "1")
                return RedirectToAction("Dashboard");
            return Redirect("/Login");
        }

        // GET: Dashboard/Dashboard
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/Login");
            return View();
        }

        // POST: Dashboard/Add_Todo
        [HttpPost]
        public async Task<IActionResult> Add_Todo(string userid, string todo_data)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { status = "error", message = "Unauthorized" });

            if (string.IsNullOrWhiteSpace(todo_data) || todo_data.Length < 5 || todo_data.Length > 150)
            {
                return Json(new { status = "error", message = "To-do Data must be between 5 and 150 characters." });
            }

            var todo = new Todo
            {
                UserId = userid,
                ToDoData = todo_data,
                Value = "1",
                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            if (_context.Entry(todo).State == EntityState.Added)
            {
                return Json(new { status = "success", message = "Successfully Added" });
            }
            else
            {
                return Json(new { status = "error", message = "Validation Error" });
            }
        }

        // POST: Dashboard/Update_Todo
        [HttpPost]
        public async Task<IActionResult> Update_Todo(int toid, string tovalue)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { status = "error", message = "Unauthorized" });

            var todo = await _context.Todos.FindAsync(toid);
            if (todo == null)
            {
                return Json(new { status = "error", message = "Todo not found" });
            }

            todo.Value = tovalue;
            _context.Update(todo);
            await _context.SaveChangesAsync();

            if (_context.Entry(todo).State == EntityState.Modified)
            {
                return Json(new { status = "success", message = "Successfully Updated" });
            }
            else
            {
                return Json(new { status = "error", message = "Something went wrong" });
            }
        }
    }
}
