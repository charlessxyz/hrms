using Microsoft.AspNetCore.Mvc;
using HRMS_CSharp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Globalization;

namespace HRMS_CSharp.Controllers
{
    public class LeaveController : Controller
    {
        private readonly HrmsContext _context;

        public LeaveController(HrmsContext context)
        {
            _context = context;
        }

        // GET: Leave/Holidays
        public async Task<IActionResult> Holidays()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var holidays = await _context.Holidays.ToListAsync();
            return View(holidays);
        }

        // GET: Leave/Holidays_for_calendar
        public async Task<IActionResult> Holidays_for_calendar()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { error = "Unauthorized" });

            var holidays = await _context.Holidays
                .Select(h => new
                {
                    title = h.HolidayName,
                    start = h.FromDate,
                    end = h.ToDate
                })
                .ToListAsync();

            return Json(holidays);
        }

        // POST: Leave/Add_Holidays
        [HttpPost]
        public async Task<IActionResult> Add_Holidays(string id, string holiname, string startdate, string enddate)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { status = "error", message = "Unauthorized" });

            if (string.IsNullOrEmpty(holiname) || holiname.Length < 5 || holiname.Length > 120)
                return Json(new { status = "error", message = "Holidays name is required and must be between 5 and 120 characters" });

            int numberOfDays = 1;
            if (!string.IsNullOrEmpty(enddate))
            {
                var start = DateTime.Parse(startdate);
                var end = DateTime.Parse(enddate);
                numberOfDays = (int)(end - start).TotalDays + 1;
            }

            string year = DateTime.Parse(startdate).ToString("MM-yyyy");

            var holiday = new Holiday
            {
                HolidayName = holiname,
                FromDate = startdate,
                ToDate = enddate,
                NumberOfDays = numberOfDays.ToString(),
                Year = year
            };

            if (string.IsNullOrEmpty(id))
            {
                _context.Holidays.Add(holiday);
                await _context.SaveChangesAsync();
                return Json(new { status = "success", message = "Successfully Added" });
            }
            else
            {
                holiday.Id = int.Parse(id);
                _context.Holidays.Update(holiday);
                await _context.SaveChangesAsync();
                return Json(new { status = "success", message = "Successfully Updated" });
            }
        }

        // POST: Leave/Add_leaves_Type
        [HttpPost]
        public async Task<IActionResult> Add_leaves_Type(string id, string leavename, string leaveday, string status)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { status = "error", message = "Unauthorized" });

            if (string.IsNullOrEmpty(leavename) || leavename.Length < 1 || leavename.Length > 220)
                return Json(new { status = "error", message = "Leave name is required and must be between 1 and 220 characters" });

            var leaveType = new LeaveType
            {
                Name = leavename,
                LeaveDay = leaveday ?? "0",
                Status = sbyte.Parse(status ?? "0")
            };

            if (string.IsNullOrEmpty(id))
            {
                _context.LeaveTypes.Add(leaveType);
                await _context.SaveChangesAsync();
                return Json(new { status = "success", message = "Successfully Added" });
            }
            else
            {
                leaveType.TypeId = int.Parse(id);
                _context.LeaveTypes.Update(leaveType);
                await _context.SaveChangesAsync();
                return Json(new { status = "success", message = "Successfully Updated" });
            }
        }

        // GET: Leave/Application
        public async Task<IActionResult> Application()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var employees = await _context.Employees.Where(e => e.EmStatus == "ACTIVE").ToListAsync();
            var leaveTypes = await _context.LeaveTypes.ToListAsync();
            var applications = await _context.EmpLeaves.ToListAsync();

            ViewBag.Employees = employees;
            ViewBag.LeaveTypes = leaveTypes;
            ViewBag.Applications = applications;

            return View();
        }

        // GET: Leave/EmApplication
        public async Task<IActionResult> EmApplication()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var emid = HttpContext.Session.GetString("user_login_id");
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == int.Parse(emid));
            var leaveTypes = await _context.LeaveTypes.ToListAsync();
            var applications = await _context.EmpLeaves.Where(e => e.EmId == emid).ToListAsync();

            ViewBag.Employee = employee;
            ViewBag.LeaveTypes = leaveTypes;
            ViewBag.Applications = applications;

            return View();
        }

        // POST: Leave/Update_Applications
        [HttpPost]
        public async Task<IActionResult> Update_Applications(string id, string emid, string typeid, string startdate, string enddate, string reason, string duration, string hour, string datetime)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { status = "error", message = "Unauthorized" });

            if (string.IsNullOrEmpty(reason) || reason.Length < 5 || reason.Length > 512)
                return Json(new { status = "error", message = "Reason is required and must be between 5 and 512 characters" });

            var application = await _context.EmpLeaves.FirstOrDefaultAsync(e => e.Id == int.Parse(id));
            if (application == null)
                return Json(new { status = "error", message = "Application not found" });

            application.EmId = emid;
            application.Typeid = int.Parse(typeid);
            application.StartDate = startdate;
            application.EndDate = enddate;
            application.Reason = reason;
            application.LeaveDuration = duration;
            application.LeaveStatus = "Approve";

            _context.EmpLeaves.Update(application);
            await _context.SaveChangesAsync();

            // Insert into AssignLeave
            var assignLeave = new AssignLeave
            {
                EmpId = int.Parse(emid),
                AppId = id,
                TypeId = int.Parse(typeid),
                Day = duration,
                Hour = hour,
                Dateyear = datetime
            };

            _context.AssignLeaves.Add(assignLeave);
            await _context.SaveChangesAsync();

            return Json(new { status = "success", message = "Successfully Approved" });
        }

        // POST: Leave/Add_Applications
        [HttpPost]
        public async Task<IActionResult> Add_Applications(string id, string emid, string typeid, string startdate, string enddate, string hourAmount, string reason, string type)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { status = "error", message = "Unauthorized" });

            if (string.IsNullOrEmpty(startdate))
                return Json(new { status = "error", message = "Start date is required" });

            double duration = 0;
            if (type == "Half Day")
            {
                duration = double.Parse(hourAmount ?? "0");
            }
            else if (type == "Full Day")
            {
                duration = 8;
            }
            else
            {
                var start = DateTime.Parse(startdate);
                var end = DateTime.Parse(enddate);
                var days = (end - start).TotalDays + 1;
                duration = days * 8;
            }

            var application = new EmpLeave
            {
                EmId = emid,
                Typeid = int.Parse(typeid),
                ApplyDate = DateTime.Now.ToString("yyyy-MM-dd"),
                StartDate = startdate,
                EndDate = enddate,
                Reason = reason,
                LeaveType = type,
                LeaveDuration = duration.ToString(),
                LeaveStatus = "Not Approve"
            };

            if (string.IsNullOrEmpty(id))
            {
                _context.EmpLeaves.Add(application);
                await _context.SaveChangesAsync();
                return Json(new { status = "success", message = "Successfully Added" });
            }
            else
            {
                application.Id = int.Parse(id);
                _context.EmpLeaves.Update(application);
                await _context.SaveChangesAsync();
                return Json(new { status = "success", message = "Successfully Updated" });
            }
        }

        // POST: Leave/Add_L_Status
        [HttpPost]
        public async Task<IActionResult> Add_L_Status(string lid, string lvalue, string duration, string type)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { status = "error", message = "Unauthorized" });

            var application = await _context.EmpLeaves.FirstOrDefaultAsync(e => e.Id == int.Parse(lid));
            if (application == null)
                return Json(new { status = "error", message = "Application not found" });

            application.LeaveStatus = lvalue;
            _context.EmpLeaves.Update(application);
            await _context.SaveChangesAsync();

            if (lvalue == "Approve")
            {
                var assignLeave = await _context.AssignLeaves.FirstOrDefaultAsync(a => a.TypeId == int.Parse(type));
                if (assignLeave != null)
                {
                    assignLeave.Day = (int.Parse(assignLeave.Day ?? "0") + int.Parse(duration)).ToString();
                    _context.AssignLeaves.Update(assignLeave);
                    await _context.SaveChangesAsync();
                }
            }

            return Json(new { status = "success", message = "Successfully Updated" });
        }

        // GET: Leave/Holidaybyib
        public async Task<IActionResult> Holidaybyib(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { error = "Unauthorized" });

            var holiday = await _context.Holidays.FirstOrDefaultAsync(h => h.Id == int.Parse(id));
            return Json(new { holidayvalue = holiday });
        }

        // GET: Leave/LeaveAppbyid
        public async Task<IActionResult> LeaveAppbyid(string id, string emid)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { error = "Unauthorized" });

            var application = await _context.EmpLeaves.FirstOrDefaultAsync(e => e.Id == int.Parse(id));
            return Json(new { leaveapplyvalue = application });
        }

        // GET: Leave/LeaveTypebYID
        public async Task<IActionResult> LeaveTypebYID(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { error = "Unauthorized" });

            var leaveType = await _context.LeaveTypes.FirstOrDefaultAsync(l => l.TypeId == int.Parse(id));
            return Json(new { leavetypevalue = leaveType });
        }

        // GET: Leave/GetEarneBalanceByEmCode
        public async Task<IActionResult> GetEarneBalanceByEmCode(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { error = "Unauthorized" });

            var earnedLeave = await _context.EarnedLeaves.FirstOrDefaultAsync(e => e.EmId == id);
            return Json(new { earnval = earnedLeave });
        }

        // GET: Leave/HOLIvalueDelet
        public async Task<IActionResult> HOLIvalueDelet(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { status = "error", message = "Unauthorized" });

            var holiday = await _context.Holidays.FirstOrDefaultAsync(h => h.Id == int.Parse(id));
            if (holiday != null)
            {
                _context.Holidays.Remove(holiday);
                await _context.SaveChangesAsync();
            }

            return Json(new { status = "success", message = "Successfully Deleted" });
        }

        // GET: Leave/APPvalueDelet
        public async Task<IActionResult> APPvalueDelet(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var application = await _context.EmpLeaves.FirstOrDefaultAsync(e => e.Id == int.Parse(id));
            if (application != null)
            {
                _context.EmpLeaves.Remove(application);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Application");
        }

        // GET: Leave/LeavetypeDelet
        public async Task<IActionResult> LeavetypeDelet(string D)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var leaveType = await _context.LeaveTypes.FirstOrDefaultAsync(l => l.TypeId == int.Parse(D));
            if (leaveType != null)
            {
                _context.LeaveTypes.Remove(leaveType);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("leavetypes");
        }

        // GET: Leave/leavetypes
        public async Task<IActionResult> leavetypes()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var leaveTypes = await _context.LeaveTypes.ToListAsync();
            return View(leaveTypes);
        }

        // GET: Leave/LeaveType
        public async Task<IActionResult> LeaveType(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { error = "Unauthorized" });

            var year = DateTime.Now.Year.ToString();
            var leaveTypes = await _context.LeaveTypes
                .Where(l => l.TypeId == int.Parse(id))
                .ToListAsync();

            var assignLeave = await _context.AssignLeaves
                .FirstOrDefaultAsync(a => a.EmpId == int.Parse(id) && a.Dateyear == year);

            string result = "";
            foreach (var leaveType in leaveTypes)
            {
                result += $"<option value='{leaveType.TypeId}'>{leaveType.Name}</option>";
            }

            if (assignLeave != null)
            {
                result += $"{assignLeave.Day}/{assignLeave.Day}";
            }

            return Content(result);
        }

        // GET: Leave/EmLeavesheet
        public async Task<IActionResult> EmLeavesheet()
        {
            var emid = HttpContext.Session.GetString("user_login_id");
            var leaveBalance = await _context.EarnedLeaves.FirstOrDefaultAsync(e => e.EmId == emid);
            ViewBag.LeaveBalance = leaveBalance;
            return View();
        }

        // GET: Leave/GetemployeeGmLeave
        public async Task<IActionResult> GetemployeeGmLeave(string year, string typeid, string emid)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { error = "Unauthorized" });

            var assignLeaves = await _context.AssignLeaves
                .Where(a => a.EmpId == int.Parse(emid) && a.TypeId == int.Parse(typeid) && a.Dateyear == year)
                .ToListAsync();

            double totalDays = assignLeaves.Sum(a => double.Parse(a.Day ?? "0"));
            var leaveType = await _context.LeaveTypes.FirstOrDefaultAsync(l => l.TypeId == int.Parse(typeid));
            string result = $"{totalDays}/{leaveType?.LeaveDay ?? "0"}";
            return Content(result);
        }

        // GET: Leave/Leave_report
        public async Task<IActionResult> Leave_report()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var employees = await _context.Employees.ToListAsync();
            ViewBag.Employees = employees;
            return View();
        }

        // GET: Leave/Get_LeaveDetails
        public async Task<IActionResult> Get_LeaveDetails(string emp_id, string date_time)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { error = "Unauthorized" });

            if (string.IsNullOrEmpty(date_time) || string.IsNullOrEmpty(emp_id))
                return Json(new { error = "Date time and employee ID are required" });

            var dateParts = date_time.Split('-');
            var day = dateParts.Length > 0 ? dateParts[0] : "";
            var year = dateParts.Length > 1 ? dateParts[1] : "";

            var leaveReports = await _context.EmpLeaves
                .Where(e => e.EmId == emp_id && e.ApplyDate.Contains(year))
                .Join(_context.LeaveTypes, el => el.Typeid, lt => lt.TypeId, (el, lt) => new
                {
                    el,
                    lt
                })
                .Join(_context.Employees, j => j.el.EmId, emp => emp.Id.ToString(), (j, emp) => new
                {
                    EmCode = emp.EmCode,
                    FirstName = emp.FirstName,
                    LastName = emp.LastName,
                    LeaveName = j.lt.Name,
                    LeaveDuration = j.el.LeaveDuration,
                    StartDate = j.el.StartDate,
                    EndDate = j.el.EndDate
                })
                .ToListAsync();

            if (!leaveReports.Any())
            {
                return Content("<p>No Data Found</p>");
            }

            string result = "";
            foreach (var report in leaveReports)
            {
                result += $"<tr><td>{report.EmCode}</td><td>{report.FirstName} {report.LastName}</td><td>{report.LeaveName}</td><td>{report.LeaveDuration} hours</td><td>{report.StartDate}</td><td>{report.EndDate}</td></tr>";
            }

            return Content(result);
        }

        // POST: Leave/approveLeaveStatus
        [HttpPost]
        public async Task<IActionResult> approveLeaveStatus(string employeeId, string lid, string lvalue, string duration, string type)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { status = "error", message = "Unauthorized" });

            var application = await _context.EmpLeaves.FirstOrDefaultAsync(e => e.Id == int.Parse(lid));
            if (application == null)
                return Json(new { status = "error", message = "Application not found" });

            application.LeaveStatus = lvalue;
            _context.EmpLeaves.Update(application);
            await _context.SaveChangesAsync();

            if (lvalue == "Approve")
            {
                var isNew = await _context.AssignLeaves.AnyAsync(a => a.EmpId == int.Parse(employeeId) && a.TypeId == int.Parse(type));
                if (isNew)
                {
                    var totalHour = await _context.AssignLeaves
                        .Where(a => a.EmpId == int.Parse(employeeId) && a.TypeId == int.Parse(type))
                        .SumAsync(a => double.Parse(a.Hour ?? "0"));

                    var assignLeave = await _context.AssignLeaves.FirstOrDefaultAsync(a => a.EmpId == int.Parse(employeeId) && a.TypeId == int.Parse(type));
                    if (assignLeave != null)
                    {
                        assignLeave.Hour = (totalHour + double.Parse(duration)).ToString();
                        _context.AssignLeaves.Update(assignLeave);
                    }
                }
                else
                {
                    var assignLeave = new AssignLeave
                    {
                        EmpId = int.Parse(employeeId),
                        TypeId = int.Parse(type),
                        Hour = duration,
                        Dateyear = DateTime.Now.Year.ToString()
                    };
                    _context.AssignLeaves.Add(assignLeave);
                }

                var earnedLeave = await _context.EarnedLeaves.FirstOrDefaultAsync(e => e.EmId == employeeId);
                if (earnedLeave != null)
                {
                    earnedLeave.PresentDate = (int.Parse(earnedLeave.PresentDate ?? "0") - (int.Parse(duration) / 8)).ToString();
                    earnedLeave.Hour = (int.Parse(earnedLeave.Hour ?? "0") - int.Parse(duration)).ToString();
                    _context.EarnedLeaves.Update(earnedLeave);
                }

                await _context.SaveChangesAsync();
            }

            return Json(new { status = "success", message = "Updated successfully" });
        }

        // GET: Leave/LeaveAssign
        public async Task<IActionResult> LeaveAssign(string employeeID, string leaveID)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { error = "Unauthorized" });

            if (!string.IsNullOrEmpty(leaveID))
            {
                var year = DateTime.Now.Year.ToString();
                var daysTaken = await _context.AssignLeaves.FirstOrDefaultAsync(a => a.EmpId == int.Parse(employeeID) && a.TypeId == int.Parse(leaveID) && a.Dateyear == year);
                var leaveType = await _context.LeaveTypes.FirstOrDefaultAsync(l => l.TypeId == int.Parse(leaveID));

                double daysTakenVal = 0;
                if (daysTaken != null && !string.IsNullOrEmpty(daysTaken.Hour))
                {
                    daysTakenVal = double.Parse(daysTaken.Hour) / 8;
                }

                string result;
                if (leaveID == "5")
                {
                    var earnedLeave = await _context.EarnedLeaves.FirstOrDefaultAsync(e => e.EmId == employeeID);
                    double earnedDays = 0;
                    if (earnedLeave != null && !string.IsNullOrEmpty(earnedLeave.Hour))
                    {
                        earnedDays = double.Parse(earnedLeave.Hour) / 8;
                    }
                    result = $"Earned Balance: {earnedDays} Days";
                }
                else
                {
                    double balance = double.Parse(leaveType?.LeaveDay ?? "0") - daysTakenVal;
                    result = $"Leave Balance: {balance} Days Of {leaveType?.LeaveDay ?? "0"}";
                }

                return Content(result);
            }

            return Content("Something wrong.");
        }

        // GET: Leave/Earnedleave
        public async Task<IActionResult> Earnedleave()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var employees = await _context.Employees.ToListAsync();
            var earnedLeaves = await _context.EarnedLeaves.ToListAsync();

            ViewBag.Employees = employees;
            ViewBag.EarnedLeaves = earnedLeaves;

            return View();
        }

        // POST: Leave/Update_Earn_Leave
        [HttpPost]
        public async Task<IActionResult> Update_Earn_Leave(string emid, string startdate, string enddate)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { status = "error", message = "Unauthorized" });

            int days = 1;
            if (!string.IsNullOrEmpty(enddate))
            {
                var start = DateTime.Parse(startdate);
                var end = DateTime.Parse(enddate);
                days = (int)(end - start).TotalDays + 1;
            }

            int hour = days * 8;
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmCode == emid);
            if (employee == null)
                return Json(new { status = "error", message = "Employee not found" });

            var earnedLeave = await _context.EarnedLeaves.FirstOrDefaultAsync(e => e.EmId == employee.Id.ToString());
            if (earnedLeave != null)
            {
                earnedLeave.PresentDate = (int.Parse(earnedLeave.PresentDate ?? "0") + days).ToString();
                earnedLeave.Hour = (int.Parse(earnedLeave.Hour ?? "0") + hour).ToString();
                earnedLeave.Status = "1";
                _context.EarnedLeaves.Update(earnedLeave);
            }
            else
            {
                earnedLeave = new EarnedLeave
                {
                    EmId = employee.Id.ToString(),
                    PresentDate = days.ToString(),
                    Hour = hour.ToString(),
                    Status = "1"
                };
                _context.EarnedLeaves.Add(earnedLeave);
            }

            await _context.SaveChangesAsync();

            // Insert attendance records
            var startDate = DateTime.Parse(startdate);
            var endDate = string.IsNullOrEmpty(enddate) ? startDate : DateTime.Parse(enddate);

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var attendance = new Attendance
                {
                    EmpId = emid,
                    AttenDate = date.ToString("yyyy-MM-dd"),
                    WorkingHour = "480",
                    SigninTime = TimeSpan.Parse("09:00:00"),
                    SignoutTime = TimeSpan.Parse("17:00:00"),
                    Status = "E"
                };
                _context.Attendances.Add(attendance);
            }

            await _context.SaveChangesAsync();

            return Json(new { status = "success", message = "Successfully Added" });
        }

        // POST: Leave/Update_Earn_Leave_Only
        [HttpPost]
        public async Task<IActionResult> Update_Earn_Leave_Only(string employee, string day, string hour)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { status = "error", message = "Unauthorized" });

            var earnedLeave = await _context.EarnedLeaves.FirstOrDefaultAsync(e => e.EmId == employee);
            if (earnedLeave != null)
            {
                earnedLeave.PresentDate = day;
                earnedLeave.Hour = hour;
                _context.EarnedLeaves.Update(earnedLeave);
                await _context.SaveChangesAsync();
            }

            return Json(new { status = "success", message = "Successfully Updated." });
        }
    }
}
