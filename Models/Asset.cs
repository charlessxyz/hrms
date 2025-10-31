using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class Asset
    {
        public int AssId { get; set; }
        public string? Catid { get; set; }
        public string? AssName { get; set; }
        public string? AssBrand { get; set; }
        public string? AssModel { get; set; }
        public string? AssCode { get; set; }
        public string? Configuration { get; set; }
        public string? PurchasingDate { get; set; }
        public string? AssPrice { get; set; }
        public string? AssQty { get; set; }
        public string? InStock { get; set; }
    }
}
