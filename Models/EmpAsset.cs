using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class EmpAsset
    {
        public int Id { get; set; }
        public int AssetsId { get; set; }
        public int EmpId { get; set; }
        public int ProjectId { get; set; }
        public int TaskId { get; set; }
        public string LogQty { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Remarks { get; set; } = null!;
    }
}
