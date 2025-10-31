using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class BankInfo
    {
        public string Id { get; set; } = null!;
        public string? AccountNumber { get; set; }
        public string? AccountType { get; set; }
        public string? BankName { get; set; }
        public string? BranchName { get; set; }
        public string? EmId { get; set; }
        public string? HolderName { get; set; }
    }
}
