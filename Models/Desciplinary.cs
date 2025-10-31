using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class Desciplinary
    {
        public string Id { get; set; } = null!;
        public string? Action { get; set; }
        public string? Description { get; set; }
        public string? EmId { get; set; }
        public string? Title { get; set; }
    }
}
