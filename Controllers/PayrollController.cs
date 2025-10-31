using Microsoft.AspNetCore.Mvc;
using HRMS_CSharp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Globalization;
using System;

namespace HRMS_CSharp.Controllers
{
    public class PayrollController : Controller
    {
        private readonly HrmsContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public PayrollController(HrmsContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // GET: Payroll/Index
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/Login");

            return RedirectToAction("Generate_salary");
        }

        // GET: Payroll/Salary_Type
        public async Task<IActionResult> Salary_Type()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/Login");

            var salaryTypes = await _context.SalaryTypes.ToListAsync();
            return View(salaryTypes);
        }

        // POST: Payroll/Add_Sallary_Type
        [HttpPost]
        public async Task<IActionResult> Add_Sallary_Type(SalaryType salaryType)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/Login");

            if (ModelState.IsValid)
            {
                salaryType.CreateDate = DateTime.Now.ToString("yyyy-MM-dd");
                _context.SalaryTypes.Add(salaryType);
                await _context.SaveChangesAsync();
                TempData["message"] = "Successfully Added";
                return Json(new { status = "success", message = "Successfully Added" });
            }
            return Json(new { status = "error", message = "Validation failed" });
        }

        // GET: Payroll/GetSallaryTypeById
        [HttpGet]
        public async Task<IActionResult> GetSallaryTypeById(int id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { error = "Unauthorized" });

            var typeValue = await _context.SalaryTypes.FindAsync(id);
            return Json(new { typevalueid = typeValue });
        }

        // GET: Payroll/Generate_salary
        public async Task<IActionResult> Generate_salary()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/Login");

            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Employees = await _context.Employees.ToListAsync();
            ViewBag.SalaryTypes = await _context.SalaryTypes.ToListAsync();
            return View();
        }

        // POST: Payroll/pay_salary_add_record
        [HttpPost]
        public async Task<IActionResult> pay_salary_add_record(PaySalary paySalary)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { status = "error", message = "Unauthorized" });

            if (ModelState.IsValid)
            {
                paySalary.PaidDate = DateTime.Now.ToString("yyyy-MM-dd");

                // Check if record exists
                var existingRecord = await _context.PaySalaries
                    .FirstOrDefaultAsync(p => p.EmpId == paySalary.EmpId && p.Month == paySalary.Month && p.Year == paySalary.Year);

                if (existingRecord != null)
                {
                    if (existingRecord.Status == "Paid")
                    {
                        return Json(new { status = "error", message = "Has already been paid" });
                    }
                    else
                    {
                        existingRecord.PaidDate = paySalary.PaidDate;
                        existingRecord.TotalDays = paySalary.TotalDays;
                        existingRecord.Basic = paySalary.Basic;
                        existingRecord.Loan = paySalary.Loan;
                        existingRecord.TotalPay = paySalary.TotalPay;
                        existingRecord.Addition = paySalary.Addition;
                        existingRecord.Diduction = paySalary.Diduction;
                        existingRecord.Status = paySalary.Status;
                        existingRecord.PaidType = paySalary.PaidType;
                        _context.Update(existingRecord);
                    }
                }
                else
                {
                    _context.PaySalaries.Add(paySalary);
                }

                await _context.SaveChangesAsync();

                // Handle loan installment if loan exists
                if (!string.IsNullOrEmpty(paySalary.Loan) && paySalary.Status == "Paid")
                {
                    var loan = await _context.Loans
                        .FirstOrDefaultAsync(l => l.EmpId == paySalary.EmpId && l.Status == "Granted");

                    if (loan != null)
                    {
                        var installment = new LoanInstallment
                        {
                            EmpId = paySalary.EmpId,
                            LoanId = loan.Id,
                            LoanNumber = loan.LoanNumber,
                            InstallAmount = paySalary.Loan,
                            AppDate = paySalary.PaidDate,
                            InstallNo = (int.Parse(loan.InstallPeriod ?? "0") - 1).ToString()
                        };
                        _context.LoanInstallments.Add(installment);

                        var loanAmount = decimal.Parse(paySalary.Loan ?? "0");
                        loan.TotalPay = (decimal.Parse(loan.TotalPay ?? "0") + loanAmount).ToString();
                        loan.TotalDue = (decimal.Parse(loan.Amount ?? "0") - decimal.Parse(loan.TotalPay ?? "0")).ToString();
                        loan.InstallPeriod = (int.Parse(loan.InstallPeriod ?? "0") - 1).ToString();

                        if (int.Parse(loan.InstallPeriod ?? "0") == 0)
                        {
                            loan.Status = "Done";
                        }

                        _context.Update(loan);
                        await _context.SaveChangesAsync();
                    }
                }

                // PayMongo integration
                if (paySalary.PaidType == "PayMongo/Gcash")
                {
                    var result = await ProcessPayMongoPayment(paySalary);
                    if (result.Contains("redirect"))
                    {
                        HttpContext.Session.SetInt32($"paymongo_{result.Split('=')[1]}_pay_id", paySalary.PayId);
                        return Json(new { status = "paymongo_redirect", checkout_url = result });
                    }
                }

                return Json(new { status = "success", message = "Successfully Added" });
            }
            return Json(new { status = "error", message = "Invalid data" });
        }

        private async Task<string> ProcessPayMongoPayment(PaySalary paySalary)
        {
            var client = _httpClientFactory.CreateClient();
            var secretKey = "sk_test_5uLMAVEfJgEkVEHG35erUvue";

            var employee = await _context.Employees.FindAsync(paySalary.EmpId);
            var amountInCentavos = decimal.Parse(paySalary.TotalPay ?? "0") * 100;
            var description = $"Salary Payment for {employee?.FirstName} {employee?.LastName}";

            var checkoutData = new
            {
                data = new
                {
                    attributes = new
                    {
                        line_items = new[]
                        {
                            new
                            {
                                currency = "PHP",
                                amount = (int)amountInCentavos,
                                description = description,
                                name = "Salary Payment",
                                quantity = 1
                            }
                        },
                        payment_method_types = new[] { "gcash" },
                        success_url = Url.Action("payment_success", "Payroll", null, Request.Scheme),
                        cancel_url = Url.Action("payment_cancel", "Payroll", null, Request.Scheme),
                        description = description
                    }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.paymongo.com/v1/checkout_sessions")
            {
                Content = new StringContent(JsonSerializer.Serialize(checkoutData), System.Text.Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{secretKey}:"))}");

            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var responseData = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var checkoutUrl = responseData.GetProperty("data").GetProperty("attributes").GetProperty("checkout_url").GetString();
                var sessionId = responseData.GetProperty("data").GetProperty("id").GetString();
                return $"{checkoutUrl}?session_id={sessionId}";
            }
            else
            {
                return "error";
            }
        }

        // GET: Payroll/Invoice/{id}
        public async Task<IActionResult> Invoice(int id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/Login");

            var salary = await _context.PaySalaries
                .Include(p => p.Emp)
                .Include(p => p.Type)
                .FirstOrDefaultAsync(p => p.PayId == id);

            if (salary == null)
            {
                return NotFound();
            }

            // Get additional data for invoice
            var employee = salary.Emp;
            var settings = await _context.Settings.FirstOrDefaultAsync();
            var bankInfo = employee != null ? await _context.BankInfos.FirstOrDefaultAsync(b => b.EmId == employee.EmId) : null;
            var addition = employee != null ? await _context.Additions.FirstOrDefaultAsync(a => a.SalaryId == employee.EmId) : null;
            var deduction = employee != null ? await _context.Deductions.FirstOrDefaultAsync(d => d.SalaryId == employee.EmId) : null;

            ViewBag.Settings = settings;
            ViewBag.BankInfo = bankInfo;
            ViewBag.Addition = addition;
            ViewBag.Deduction = deduction;

            return View(salary);
        }

        // AJAX: Load employee by department ID for payroll
        [HttpGet]
        public async Task<IActionResult> load_employee_by_deptID_for_pay(string date_time, int dep_id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Content("Unauthorized");

            var orderDate = date_time.Split('-');
            var month = orderDate[0];
            var year = orderDate[1];

            var employees = await _context.Employees
                .Where(e => e.DepId == dep_id)
                .Include(e => e.EmpSalary)
                .ToListAsync();

            // Calculate working days and hours
            var daysInMonth = DateTime.DaysInMonth(int.Parse(year), int.Parse(month));
            var holidayRecords = await _context.Holidays
                .Where(h => h.Year == date_time)
                .ToListAsync();
            var holidays = holidayRecords.Sum(h => int.Parse(h.NumberOfDays ?? "0"));

            var fridays = 0;
            for (int i = 1; i <= daysInMonth; i++)
            {
                var date = new DateTime(int.Parse(year), int.Parse(month), i);
                if (date.DayOfWeek == DayOfWeek.Friday) fridays++;
            }

            var totalDaysOff = holidays + fridays;
            var totalWorkDays = daysInMonth - totalDaysOff;
            var totalWorkHours = totalWorkDays * 8;

            var html = "";
            foreach (var emp in employees)
            {
                var hourRate = decimal.Parse(emp.EmpSalary?.Total ?? "0") / totalWorkHours;
                html += $"<tr><td>{emp.EmCode}</td><td>{emp.FirstName} {emp.LastName}</td><td>{emp.EmpSalary?.Total ?? "0"}</td><td>{hourRate:F2}</td><td>{totalWorkHours}</td><td><a href='' data-id='{emp.EmId}' class='btn btn-sm btn-danger waves-effect waves-light salaryGenerateModal' data-toggle='modal' data-target='#SalaryTypemodel' data-hour='{totalWorkHours}'>Generate Salary</a></td></tr>";
            }

            return Content(html, "text/html");
        }

        // AJAX: Generate payroll for each employee
        [HttpGet]
        public async Task<IActionResult> generate_payroll_for_each_employee(int month, int year, string employeeID)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { error = "Unauthorized" });

            var employee = await _context.Employees
                .Include(e => e.EmpSalary)
                .FirstOrDefaultAsync(e => e.EmId == employeeID);

            if (employee == null)
            {
                return Json(new { error = "Employee not found" });
            }

            // Calculate hours worked
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var attendanceHours = await _context.Attendances
                .Where(a => a.EmpId == employeeID && a.AttenDate != null && DateTime.Parse(a.AttenDate) >= startDate && DateTime.Parse(a.AttenDate) <= endDate)
                .Select(a => TimeSpan.Parse(a.WorkingHour ?? "00:00:00").TotalHours)
                .SumAsync();

            // Get loan info
            var loan = await _context.Loans
                .FirstOrDefaultAsync(l => l.EmpId == employeeID && l.Status == "Granted");

            // Get additions and deductions
            var addition = await _context.Additions.FirstOrDefaultAsync(a => a.SalaryId == employeeID);
            var deduction = await _context.Deductions.FirstOrDefaultAsync(d => d.SalaryId == employeeID);

            var basicSalary = decimal.Parse(employee.EmpSalary?.Total ?? "0");
            var totalWorkHours = 160; // Assuming 160 working hours per month
            var hourlyRate = basicSalary / totalWorkHours;
            var employeeWorkedHours = (decimal)attendanceHours;

            var additionAmount = decimal.Parse(addition?.Basic ?? "0") +
                                decimal.Parse(addition?.Conveyance ?? "0") +
                                decimal.Parse(addition?.HouseRent ?? "0") +
                                decimal.Parse(addition?.Medical ?? "0");

            var deductionAmount = decimal.Parse(deduction?.Tax ?? "0") +
                                 decimal.Parse(deduction?.Bima ?? "0") +
                                 decimal.Parse(deduction?.ProvidentFund ?? "0") +
                                 decimal.Parse(deduction?.Others ?? "0");

            var loanAmount = decimal.Parse(loan?.Installment ?? "0");
            var workHourDiff = totalWorkHours - (double)employeeWorkedHours;

            decimal additionalPay = 0;
            decimal deductionPay = 0;

            if (workHourDiff < 0)
            {
                additionalPay = (decimal)Math.Abs(workHourDiff) * hourlyRate;
            }
            else if (workHourDiff > 0)
            {
                deductionPay = (decimal)Math.Abs(workHourDiff) * hourlyRate;
            }

            var finalSalary = basicSalary + additionAmount - deductionAmount - loanAmount + additionalPay - deductionPay;

            var response = new
            {
                basic_salary = basicSalary.ToString("F2"),
                total_work_hours = totalWorkHours,
                employee_actually_worked = employeeWorkedHours.ToString("F2"),
                wpay = workHourDiff.ToString("F2"),
                addition = (additionAmount + additionalPay).ToString("F2"),
                diduction = (deductionAmount + deductionPay).ToString("F2"),
                loan_amount = loanAmount.ToString("F2"),
                loan_id = loan?.Id.ToString() ?? "",
                final_salary = finalSalary.ToString("F2"),
                rate = hourlyRate.ToString("F2")
            };

            return Json(response);
        }

        // PayMongo payment success callback
        public async Task<IActionResult> payment_success(string session_id)
        {
            if (string.IsNullOrEmpty(session_id))
            {
                return RedirectToAction("Generate_salary");
            }

            // Retrieve pay_id from session
            var payId = HttpContext.Session.GetInt32($"paymongo_{session_id}_pay_id");
            if (payId == null)
            {
                return Content("Session expired or invalid.");
            }

            var success = await UpdatePaidSalaryData(payId.Value, "Paid");
            if (success)
            {
                HttpContext.Session.Remove($"paymongo_{session_id}_pay_id");
                return RedirectToAction("Generate_salary");
            }
            else
            {
                return Content("Error updating payment status.");
            }
        }

        // PayMongo payment cancel callback
        public IActionResult payment_cancel(string session_id)
        {
            if (!string.IsNullOrEmpty(session_id))
            {
                var payId = HttpContext.Session.GetInt32($"paymongo_{session_id}_pay_id");
                if (payId != null)
                {
                    UpdatePaidSalaryData(payId.Value, "Cancelled");
                }
                HttpContext.Session.Remove($"paymongo_{session_id}_pay_id");
            }
            return Content("Payment cancelled.");
        }

        private async Task<bool> UpdatePaidSalaryData(int payId, string status)
        {
            var paySalary = await _context.PaySalaries.FindAsync(payId);
            if (paySalary != null)
            {
                paySalary.Status = status;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // GET: Payroll/Salary_List
        public async Task<IActionResult> Salary_List()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/Login");

            var salaryData = await _context.PaySalaries
                .Include(p => p.Emp)
                .OrderByDescending(p => p.Month)
                .ToListAsync();

            return View(salaryData);
        }

        // GET: Payroll/Payslip_Report
        public async Task<IActionResult> Payslip_Report()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/Login");

            ViewBag.Employees = await _context.Employees.ToListAsync();
            return View();
        }
    }
}
