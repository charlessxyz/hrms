using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class LoanInstallment
    {
        public int Id { get; set; }
        public int LoanId { get; set; }
        public string? EmpId { get; set; }
        public string? LoanNumber { get; set; }
        public string? InstallAmount { get; set; }
        public string? PayAmount { get; set; }
        public string? AppDate { get; set; }
        public string? Receiver { get; set; }
        public string? InstallNo { get; set; }
        public string? Notes { get; set; }
    }
}
