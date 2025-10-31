using Microsoft.AspNetCore.Mvc;
using HRMS_CSharp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace HRMS_CSharp.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly HrmsContext _context;

        public AttendanceController(HrmsContext context)
        {
            _context = context;
        }

        // GET: Attendance/Attendance
        public async Task<IActionResult> Attendance()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var attendances = await _context.Attendances.ToListAsync();
            return View(attendances);
        }

        // GET: Attendance/Save_Attendance
        public async Task<IActionResult> Save_Attendance(string A)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var employees = await _context.Employees.ToListAsync();
            ViewBag.Employees = employees;

            if (!string.IsNullOrEmpty(A))
            {
                var attval = await _context.Attendances.FirstOrDefaultAsync(a => a.Id == int.Parse(A));
                ViewBag.Attval = attval;
            }

            return View();
        }

        // GET: Attendance/Attendance_Report
        public async Task<IActionResult> Attendance_Report(string A)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var employees = await _context.Employees.ToListAsync();
            ViewBag.Employees = employees;

            if (!string.IsNullOrEmpty(A))
            {
                var attval = await _context.Attendances.FirstOrDefaultAsync(a => a.Id == int.Parse(A));
                ViewBag.Attval = attval;
            }

            return View();
        }

        // POST: Attendance/Get_attendance_data_for_report
        [HttpPost]
        public async Task<IActionResult> Get_attendance_data_for_report(string date_from, string date_to, string employee_id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { error = "Unauthorized" });

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == int.Parse(employee_id));
            if (employee == null) return Json(new { error = "Employee not found" });

            var attendanceData = await _context.Attendances
                .Where(a => a.EmpId == employee_id && DateTime.Parse(a.AttenDate) >= DateTime.Parse(date_from) && DateTime.Parse(a.AttenDate) <= DateTime.Parse(date_to))
                .ToListAsync();

            var totalHours = attendanceData.Sum(a => TimeSpan.Parse(a.WorkingHour ?? "00:00:00").TotalHours);

            return Json(new
            {
                attendance = attendanceData,
                name = $"{employee.FirstName} {employee.LastName}",
                days = attendanceData.Count,
                hours = totalHours
            });
        }

        // POST: Attendance/Add_Attendance
        [HttpPost]
        public async Task<IActionResult> Add_Attendance(Attendance attendance, string attdate, string signin, string signout, string place)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { status = "error", message = "Unauthorized" });

            if (!ModelState.IsValid)
                return Json(new { status = "error", message = "Validation failed" });

            var attDate = DateTime.Parse(attdate);
            var signIn = TimeSpan.Parse(signin);
            var signOut = TimeSpan.Parse(signout);
            var workingHour = signOut - signIn;

            attendance.AttenDate = attDate.ToString("yyyy-MM-dd");
            attendance.SigninTime = TimeSpan.Parse(signin);
            attendance.SignoutTime = TimeSpan.Parse(signout);
            attendance.WorkingHour = workingHour.ToString(@"hh\:mm\:ss");
            attendance.Place = place;
            attendance.Status = "A";

            var dayOfWeek = attDate.DayOfWeek;
            var isHoliday = await _context.Holidays.AnyAsync(h => h.FromDate == attDate.ToString("yyyy-MM-dd"));

            if (attendance.Id == 0)
            {
                var duplicate = await _context.Attendances.AnyAsync(a => a.EmpId == attendance.EmpId && a.AttenDate == attDate.ToString("yyyy-MM-dd"));
                if (duplicate)
                    return Json(new { status = "error", message = "Already Exist" });

                if (dayOfWeek == DayOfWeek.Friday || isHoliday)
                {
                    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == int.Parse(attendance.EmpId));
                    if (employee != null)
                    {
                        var earnedLeave = await _context.EarnedLeaves.FirstOrDefaultAsync(el => el.EmId == employee.Id.ToString());
                        if (earnedLeave != null)
                        {
                            earnedLeave.PresentDate = (int.Parse(earnedLeave.PresentDate ?? "0") + 1).ToString();
                            earnedLeave.Hour = (int.Parse(earnedLeave.Hour ?? "0") + 480).ToString();
                            earnedLeave.Status = "1";
                            _context.EarnedLeaves.Update(earnedLeave);
                        }
                    }
                    attendance.Status = "E";
                }

                _context.Attendances.Add(attendance);
            }
            else
            {
                _context.Attendances.Update(attendance);
            }

            await _context.SaveChangesAsync();
            return Json(new { status = "success", message = "Successfully Added/Updated" });
        }

        // POST: Attendance/import
        [HttpPost]
        public async Task<IActionResult> import(IFormFile csv_file)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { status = "error", message = "Unauthorized" });

            if (csv_file == null || csv_file.Length == 0)
                return Json(new { status = "error", message = "No file uploaded" });

            // Simplified CSV import without CsvHelper
            using (var reader = new StreamReader(csv_file.OpenReadStream()))
            {
                string line;
                bool isFirstLine = true;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        continue; // Skip header
                    }

                    var columns = line.Split(',');
                    if (columns.Length >= 8)
                    {
                        var checkIn = columns[1]; // Assuming Check-in at is second column
                        if (!string.IsNullOrEmpty(checkIn) && checkIn != "0:00:00")
                        {
                            var date = DateTime.Parse(columns[0]); // Date
                            var empId = columns[2]; // Employee No

                            var duplicate = await _context.Attendances.AnyAsync(a => a.EmpId == empId && a.AttenDate == date.ToString("yyyy-MM-dd"));

                            var attendance = new Attendance
                            {
                                EmpId = empId,
                                AttenDate = date.ToString("yyyy-MM-dd"),
                                SigninTime = TimeSpan.Parse(checkIn),
                                SignoutTime = TimeSpan.Parse(columns[3]), // Check-out at
                                WorkingHour = columns[4], // Work Duration
                                Absence = columns[5], // Absence Duration
                                Overtime = columns[6], // Overtime Duration
                                Status = "A",
                                Place = "office"
                            };

                            if (duplicate)
                            {
                                // Update existing
                                var existing = await _context.Attendances.FirstOrDefaultAsync(a => a.EmpId == empId && a.AttenDate == date.ToString("yyyy-MM-dd"));
                                if (existing != null)
                                {
                                    existing.SigninTime = attendance.SigninTime;
                                    existing.SignoutTime = attendance.SignoutTime;
                                    existing.WorkingHour = attendance.WorkingHour;
                                    existing.Absence = attendance.Absence;
                                    existing.Overtime = attendance.Overtime;
                                    _context.Attendances.Update(existing);
                                }
                            }
                            else
                            {
                                _context.Attendances.Add(attendance);
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }

            return Json(new { status = "success", message = "Successfully Updated" });
        }
    }
}
