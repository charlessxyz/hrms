using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class PaySalary
    {
        public int PayId { get; set; }
        public string? EmpId { get; set; }
        public int? TypeId { get; set; }
        public string? Month { get; set; }
        public string? Year { get; set; }
        public string? PaidDate { get; set; }
        public string? TotalDays { get; set; }
        public string? Basic { get; set; }
        public string? Medical { get; set; }
        public string? HouseRent { get; set; }
        public string? Bonus { get; set; }
        public string? Bima { get; set; }
        public string? Tax { get; set; }
        public string? ProvidentFund { get; set; }
        public string? Loan { get; set; }
        public string? TotalPay { get; set; }
        public string? Addition { get; set; }
        public string? Diduction { get; set; }
        public string? Status { get; set; }
        public string? PaidType { get; set; }

        public virtual Employee? Emp { get; set; }
        public virtual SalaryType? Type { get; set; }
    }
}
