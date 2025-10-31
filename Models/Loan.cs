using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class Loan
    {
        public int Id { get; set; }
        public string? EmpId { get; set; }
        public string? Amount { get; set; }
        public string? InterestPercentage { get; set; }
        public string? TotalAmount { get; set; }
        public string? TotalPay { get; set; }
        public string? TotalDue { get; set; }
        public string? Installment { get; set; }
        public string? LoanNumber { get; set; }
        public string? LoanDetails { get; set; }
        public string? ApproveDate { get; set; }
        public string? InstallPeriod { get; set; }
        public string Status { get; set; } = null!;
    }
}
