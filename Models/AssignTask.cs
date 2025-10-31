using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class AssignTask
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int ProId { get; set; }
        public int AssignUser { get; set; }
        public string UserType { get; set; } = null!;
    }
}
