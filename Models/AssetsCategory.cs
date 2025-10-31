using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class AssetsCategory
    {
        public int CatId { get; set; }
        public string CatStatus { get; set; } = null!;
        public string? CatName { get; set; }
    }
}
