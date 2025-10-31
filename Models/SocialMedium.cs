using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class SocialMedium
    {
        public string Id { get; set; } = null!;
        public string? EmpId { get; set; }
        public string? Facebook { get; set; }
        public string? GooglePlus { get; set; }
        public string? SkypeId { get; set; }
        public string? Twitter { get; set; }
    }
}
