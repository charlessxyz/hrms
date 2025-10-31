using Microsoft.AspNetCore.Mvc;
using HRMS_CSharp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace HRMS_CSharp.Controllers
{
    public class LoanController : Controller
    {
        private readonly HrmsContext _context;

        public LoanController(HrmsContext context)
        {
            _context = context;
        }

        // GET: Loan/View
        public async Task<IActionResult> View()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var employees = await _context.Employees.ToListAsync();
            var loans = await _context.Loans.ToListAsync();
            ViewBag.Employees = employees;
            return View(loans);
        }

        // POST: Loan/Add_Loan
        [HttpPost]
        public async Task<IActionResult> Add_Loan(Loan loan)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { status = "error", message = "Unauthorized" });

            if (!ModelState.IsValid)
            {
                return Json(new { status = "error", message = "Validation failed" });
            }

            var existingLoan = await _context.Loans
                .FirstOrDefaultAsync(l => l.EmpId == loan.EmpId && l.Status == "Granted");

            if (existingLoan != null && loan.Id == 0)
            {
                return Json(new { status = "error", message = "Already you have a loan. Please pay installation first" });
            }

            loan.TotalPay = "0";
            loan.TotalDue = "0";

            if (loan.Id != 0)
            {
                _context.Loans.Update(loan);
            }
            else
            {
                _context.Loans.Add(loan);
            }
            await _context.SaveChangesAsync();

            return Json(new { status = "success", message = "Successfully Added/Updated" });
        }

        // GET: Loan/installment
        public async Task<IActionResult> installment()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var employees = await _context.Employees.ToListAsync();
            var installments = await _context.LoanInstallments.ToListAsync();
            ViewBag.Employees = employees;
            return View(installments);
        }

        // POST: Loan/Add_Loan_Installment
        [HttpPost]
        public async Task<IActionResult> Add_Loan_Installment(LoanInstallment installment)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { status = "error", message = "Unauthorized" });

            if (!ModelState.IsValid)
            {
                return Json(new { status = "error", message = "Validation failed" });
            }

            if (installment.Id != 0)
            {
                _context.LoanInstallments.Update(installment);
            }
            else
            {
                var loan = await _context.Loans.FindAsync(installment.LoanId);
                if (loan != null)
                {
                    var period = int.Parse(loan.InstallPeriod ?? "0") - 1;
                    installment.InstallNo = period.ToString();

                    _context.LoanInstallments.Add(installment);

                    var amount = decimal.Parse(installment.InstallAmount ?? "0");
                    loan.TotalPay = (decimal.Parse(loan.TotalPay ?? "0") + amount).ToString();
                    loan.TotalDue = (decimal.Parse(loan.Amount ?? "0") - decimal.Parse(loan.TotalPay ?? "0")).ToString();
                    loan.InstallPeriod = period.ToString();

                    if (period == 0)
                    {
                        loan.Status = "Done";
                    }

                    _context.Loans.Update(loan);
                }
            }
            await _context.SaveChangesAsync();

            return Json(new { status = "success", message = "Successfully Added/Updated" });
        }

        // GET: Loan/LoanByID
        [HttpGet]
        public async Task<IActionResult> LoanByID(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Json(new { error = "Unauthorized" });

            var loanId = int.Parse(id);
            var loan = await _context.Loans.FindAsync(loanId);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == int.Parse(loan.EmpId));
            var installments = await _context.LoanInstallments.Where(li => li.LoanId == loanId).ToListAsync();

            return Json(new { loanvalue = loan, loanvalueem = employee, loanvalueinstallment = installments });
        }
    }
}
