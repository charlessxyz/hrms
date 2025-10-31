using System;

namespace HRMS_CSharp.Models
{
    public partial class ProjectExpense
    {
        public int Id { get; set; }
        public int ProId { get; set; }
        public string Details { get; set; } = null!;
        public decimal Amount { get; set; }
        public int AssignTo { get; set; }
        public DateTime Date { get; set; }
    }
}
