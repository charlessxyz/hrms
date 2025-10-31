using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class Training
    {
        public int Id { get; set; }
        public string? TrainingName { get; set; }
        public string? TrainingDate { get; set; }
        public string? TrainingDuration { get; set; }
        public string? TrainingCost { get; set; }
    }
}
