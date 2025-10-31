using Microsoft.AspNetCore.Mvc;
using HRMS_CSharp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

namespace HRMS_CSharp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly HrmsContext _context;

        public EmployeeController(HrmsContext context)
        {
            _context = context;
        }

        // GET: Employee/Index
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return RedirectToAction("Index", "Login");

            return RedirectToAction("Employees");
        }

        // GET: Employee/Employees
        public async Task<IActionResult> Employees()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var employees = await _context.Employees.ToListAsync();
            return View(employees);
        }

        // GET: Employee/Add_employee
        public IActionResult Add_employee()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            return View();
        }

        // POST: Employee/Save
        [HttpPost]
        public async Task<IActionResult> Save(Employee employee, IFormFile image_url)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (!ModelState.IsValid)
            {
                return Content("Validation errors");
            }

            var existingEmail = await _context.Employees.AnyAsync(e => e.EmEmail == employee.EmEmail);
            if (existingEmail && string.IsNullOrEmpty(employee.EmId))
            {
                return Content("Email is already Exist");
            }

            employee.EmId = $"{employee.LastName?.Substring(0, 3)}{new Random().Next(1000, 2000)}";
            employee.EmPassword = HashPassword(employee.EmPhone ?? "");
            employee.EmStatus = "ACTIVE";

            if (image_url != null)
            {
                var fileName = $"{employee.EmId}_{Path.GetFileName(image_url.FileName)}";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "users", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image_url.CopyToAsync(stream);
                }
                employee.EmImage = fileName;
            }

            if (!string.IsNullOrEmpty(employee.EmId))
            {
                _context.Employees.Update(employee);
            }
            else
            {
                _context.Employees.Add(employee);
            }
            await _context.SaveChangesAsync();

            return Content("Successfully Added/Updated");
        }

        // POST: Employee/Update
        [HttpPost]
        public async Task<IActionResult> Update(Employee employee, IFormFile image_url)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (!ModelState.IsValid)
            {
                return Content("Validation errors");
            }

            var existingEmployee = await _context.Employees.FindAsync(employee.Id);
            if (existingEmployee == null)
            {
                return Content("Employee not found");
            }

            if (image_url != null)
            {
                var fileName = $"{employee.EmId}_{Path.GetFileName(image_url.FileName)}";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "users", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image_url.CopyToAsync(stream);
                }
                employee.EmImage = fileName;
            }

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return Content("Successfully Updated");
        }

        // GET: Employee/View/{id}
        public async Task<IActionResult> View(string I)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var id = I; // Assuming base64 decode if needed, but for simplicity
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            // Additional data like addresses, education, etc. would be loaded here
            return View(employee);
        }

        // POST: Employee/Parmanent_Address
        [HttpPost]
        public async Task<IActionResult> Parmanent_Address(Address address)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (!ModelState.IsValid)
            {
                return Content("Validation errors");
            }

            address.Type = "Permanent";
            if (!string.IsNullOrEmpty(address.Id))
            {
                _context.Addresses.Update(address);
            }
            else
            {
                _context.Addresses.Add(address);
            }
            await _context.SaveChangesAsync();

            return Content("Successfully Added/Updated");
        }

        // POST: Employee/Present_Address
        [HttpPost]
        public async Task<IActionResult> Present_Address(Address address)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (!ModelState.IsValid)
            {
                return Content("Validation errors");
            }

            address.Type = "Present";
            if (!string.IsNullOrEmpty(address.Id))
            {
                _context.Addresses.Update(address);
            }
            else
            {
                _context.Addresses.Add(address);
            }
            await _context.SaveChangesAsync();

            return Content("Successfully Added/Updated");
        }

        // POST: Employee/Add_Education
        [HttpPost]
        public async Task<IActionResult> Add_Education(Education education)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (!ModelState.IsValid)
            {
                return Content("Validation errors");
            }

            if (!string.IsNullOrEmpty(education.Id))
            {
                _context.Educations.Update(education);
            }
            else
            {
                _context.Educations.Add(education);
            }
            await _context.SaveChangesAsync();

            return Content("Successfully Added/Updated");
        }

        // POST: Employee/Save_Social
        [HttpPost]
        public async Task<IActionResult> Save_Social(SocialMedium social)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (!string.IsNullOrEmpty(social.Id))
            {
                _context.SocialMedia.Update(social);
            }
            else
            {
                _context.SocialMedia.Add(social);
            }
            await _context.SaveChangesAsync();

            return Content("Successfully Added/Updated");
        }

        // POST: Employee/Add_Experience
        [HttpPost]
        public async Task<IActionResult> Add_Experience(EmpExperience experience)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (!ModelState.IsValid)
            {
                return Content("Validation errors");
            }

            if (!string.IsNullOrEmpty(experience.Id))
            {
                _context.EmpExperiences.Update(experience);
            }
            else
            {
                _context.EmpExperiences.Add(experience);
            }
            await _context.SaveChangesAsync();

            return Content("Successfully Added/Updated");
        }

        // GET: Employee/Disciplinary
        public async Task<IActionResult> Disciplinary()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var disciplinary = await _context.Desciplinaries.ToListAsync();
            return View(disciplinary);
        }

        // POST: Employee/add_Desciplinary
        [HttpPost]
        public async Task<IActionResult> add_Desciplinary(Desciplinary disciplinary)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (!ModelState.IsValid)
            {
                return Content("Validation errors");
            }

            if (!string.IsNullOrEmpty(disciplinary.Id))
            {
                _context.Desciplinaries.Update(disciplinary);
            }
            else
            {
                _context.Desciplinaries.Add(disciplinary);
            }
            await _context.SaveChangesAsync();

            return Content("Successfully Added/Updated");
        }

        // POST: Employee/Add_bank_info
        [HttpPost]
        public async Task<IActionResult> Add_bank_info(BankInfo bankInfo)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (!ModelState.IsValid)
            {
                return Content("Validation errors");
            }

            if (!string.IsNullOrEmpty(bankInfo.Id))
            {
                _context.BankInfos.Update(bankInfo);
            }
            else
            {
                _context.BankInfos.Add(bankInfo);
            }
            await _context.SaveChangesAsync();

            return Content("Successfully Added/Updated");
        }

        // POST: Employee/Reset_Password_Hr
        [HttpPost]
        public async Task<IActionResult> Reset_Password_Hr(string emid, string new1, string new2)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (new1 != new2)
            {
                return Content("Please enter valid password");
            }

            var employee = await _context.Employees.FindAsync(emid);
            if (employee != null)
            {
                employee.EmPassword = HashPassword(new1);
                await _context.SaveChangesAsync();
                return Content("Successfully Updated");
            }
            return Content("Employee not found");
        }

        // POST: Employee/Reset_Password
        [HttpPost]
        public async Task<IActionResult> Reset_Password(string emid, string old, string new1, string new2)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var employee = await _context.Employees.FindAsync(emid);
            if (employee != null && employee.EmPassword == HashPassword(old))
            {
                if (new1 == new2)
                {
                    employee.EmPassword = HashPassword(new1);
                    await _context.SaveChangesAsync();
                    return Content("Successfully Updated");
                }
                else
                {
                    return Content("Please enter valid password");
                }
            }
            return Content("Please enter valid password");
        }

        // GET: Employee/Department
        public async Task<IActionResult> Department()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var departments = await _context.Departments.ToListAsync();
            return View(departments);
        }

        // POST: Employee/Save_dep
        [HttpPost]
        public async Task<IActionResult> Save_dep(Department department)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (!ModelState.IsValid)
            {
                return Content("Validation errors");
            }

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return Content("Successfully Added");
        }

        // GET: Employee/Designation
        public async Task<IActionResult> Designation()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var designations = await _context.Designations.ToListAsync();
            return View(designations);
        }

        // POST: Employee/Des_Save
        [HttpPost]
        public async Task<IActionResult> Des_Save(Designation designation)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (!ModelState.IsValid)
            {
                return Content("Validation errors");
            }

            _context.Designations.Add(designation);
            await _context.SaveChangesAsync();

            return Content("Successfully Added");
        }

        // POST: Employee/Assign_leave
        [HttpPost]
        public async Task<IActionResult> Assign_leave(AssignLeave assignLeave)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (!ModelState.IsValid)
            {
                return Content("Validation errors");
            }

            _context.AssignLeaves.Add(assignLeave);
            await _context.SaveChangesAsync();

            return Content("Successfully Added");
        }

        // POST: Employee/Add_File
        [HttpPost]
        public async Task<IActionResult> Add_File(EmployeeFile employeeFile, IFormFile file_url)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (!ModelState.IsValid)
            {
                return Content("Validation errors");
            }

            if (file_url != null)
            {
                var fileName = $"{employeeFile.EmId}_{Path.GetFileName(file_url.FileName)}";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "users", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file_url.CopyToAsync(stream);
                }
                employeeFile.FileUrl = fileName;
            }

            _context.EmployeeFiles.Add(employeeFile);
            await _context.SaveChangesAsync();

            return Content("Successfully Updated");
        }

        // GET: Employee/educationbyib
        [HttpGet]
        public async Task<IActionResult> educationbyib(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var education = await _context.Educations.Where(e => e.Id == id).FirstOrDefaultAsync();
            return Json(new { educationvalue = education });
        }

        // GET: Employee/experiencebyib
        [HttpGet]
        public async Task<IActionResult> experiencebyib(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var experience = await _context.EmpExperiences.Where(e => e.Id == id).FirstOrDefaultAsync();
            return Json(new { expvalue = experience });
        }

        // GET: Employee/DisiplinaryByID
        [HttpGet]
        public async Task<IActionResult> DisiplinaryByID(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var disciplinary = await _context.Desciplinaries.Where(d => d.Id == id).FirstOrDefaultAsync();
            return Json(new { desipplinary = disciplinary });
        }

        // GET: Employee/EduvalueDelet
        [HttpGet]
        public async Task<IActionResult> EduvalueDelet(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var education = await _context.Educations.FindAsync(id);
            if (education != null)
            {
                _context.Educations.Remove(education);
                await _context.SaveChangesAsync();
            }
            return Content("Successfully Deleted");
        }

        // GET: Employee/EXPvalueDelet
        [HttpGet]
        public async Task<IActionResult> EXPvalueDelet(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var experience = await _context.EmpExperiences.FindAsync(id);
            if (experience != null)
            {
                _context.EmpExperiences.Remove(experience);
                await _context.SaveChangesAsync();
            }
            return Content("Successfully Deleted");
        }

        // GET: Employee/DeletDisiplinary
        [HttpGet]
        public async Task<IActionResult> DeletDisiplinary(string D)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var disciplinary = await _context.Desciplinaries.FindAsync(D);
            if (disciplinary != null)
            {
                _context.Desciplinaries.Remove(disciplinary);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Disciplinary");
        }

        // POST: Employee/Add_Salary
        [HttpPost]
        public async Task<IActionResult> Add_Salary(EmpSalary empSalary, Addition addition, Deduction deduction)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (!ModelState.IsValid)
            {
                return Content("Validation errors");
            }

            if (!string.IsNullOrEmpty(empSalary.Id))
            {
                _context.EmpSalaries.Update(empSalary);
                if (!string.IsNullOrEmpty(addition.AddiId))
                {
                    _context.Additions.Update(addition);
                }
                if (!string.IsNullOrEmpty(deduction.DeId))
                {
                    _context.Deductions.Update(deduction);
                }
            }
            else
            {
                _context.EmpSalaries.Add(empSalary);
                _context.Additions.Add(addition);
                _context.Deductions.Add(deduction);
            }
            await _context.SaveChangesAsync();

            return Content("Successfully Added/Updated");
        }

        // GET: Employee/Inactive_Employee
        public async Task<IActionResult> Inactive_Employee()
        {
            var invalidUsers = await _context.Employees.Where(e => e.EmStatus != "ACTIVE").ToListAsync();
            return View(invalidUsers);
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
