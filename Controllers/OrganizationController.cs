using Microsoft.AspNetCore.Mvc;
using HRMS_CSharp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HRMS_CSharp.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly HrmsContext _context;

        public OrganizationController(HrmsContext context)
        {
            _context = context;
        }

        // GET: Organization/Index
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            return Redirect("/Dashboard/Dashboard");
        }

        // GET: Organization/Department
        public async Task<IActionResult> Department()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var departments = await _context.Departments.ToListAsync();
            return View(departments);
        }

        // POST: Organization/Save_dep
        [HttpPost]
        public async Task<IActionResult> Save_dep(string department)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (string.IsNullOrEmpty(department) || department.Length < 2 || department.Length > 220)
                return Content("Department name must be between 2 and 220 characters");

            var data = new Department
            {
                DepName = department
            };

            _context.Departments.Add(data);
            await _context.SaveChangesAsync();
            return Content("Successfully Added");
        }

        // GET: Organization/Delete_dep
        [HttpGet]
        public async Task<IActionResult> Delete_dep(int dep_id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var department = await _context.Departments.FindAsync(dep_id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Department");
        }

        // GET: Organization/Dep_edit
        [HttpGet]
        public async Task<IActionResult> Dep_edit(int dep)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var departments = await _context.Departments.ToListAsync();
            var editDepartment = await _context.Departments.FindAsync(dep);
            ViewBag.EditDepartment = editDepartment;
            return View("Department", departments);
        }

        // POST: Organization/Update_dep
        [HttpPost]
        public async Task<IActionResult> Update_dep(int id, string department)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var data = new Department
            {
                Id = id,
                DepName = department
            };

            _context.Departments.Update(data);
            await _context.SaveChangesAsync();
            return Content("Successfully Updated");
        }

        // GET: Organization/Designation
        public async Task<IActionResult> Designation()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var designations = await _context.Designations.ToListAsync();
            return View(designations);
        }

        // POST: Organization/Save_des
        [HttpPost]
        public async Task<IActionResult> Save_des(string designation)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (string.IsNullOrEmpty(designation) || designation.Length < 2 || designation.Length > 220)
                return Content("Designation name must be between 2 and 220 characters");

            var data = new Designation
            {
                DesName = designation
            };

            _context.Designations.Add(data);
            await _context.SaveChangesAsync();
            return Content("Successfully Added");
        }

        // GET: Organization/des_delete
        [HttpGet]
        public async Task<IActionResult> des_delete(int des_id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var designation = await _context.Designations.FindAsync(des_id);
            if (designation != null)
            {
                _context.Designations.Remove(designation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Designation");
        }

        // GET: Organization/Edit_des
        [HttpGet]
        public async Task<IActionResult> Edit_des(int des)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var designations = await _context.Designations.ToListAsync();
            var editDesignation = await _context.Designations.FindAsync(des);
            ViewBag.EditDesignation = editDesignation;
            return View("Designation", designations);
        }

        // POST: Organization/Update_des
        [HttpPost]
        public async Task<IActionResult> Update_des(int id, string designation)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var data = new Designation
            {
                Id = id,
                DesName = designation
            };

            _context.Designations.Update(data);
            await _context.SaveChangesAsync();
            return Content("Successfully Updated");
        }
    }
}
