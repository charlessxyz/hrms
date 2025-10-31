using System;
using System.Collections.Generic;

namespace HRMS_CSharp.Models
{
    public partial class Employee
    {
        public int Id { get; set; }
        public string? EmId { get; set; }
        public string? EmCode { get; set; }
        public int? DesId { get; set; }
        public int? DepId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EmEmail { get; set; }
        public string EmPassword { get; set; } = null!;
        public string EmRole { get; set; } = null!;
        public string? EmAddress { get; set; }
        public string EmStatus { get; set; } = null!;
        public string EmGender { get; set; } = null!;
        public string? EmPhone { get; set; }
        public string? EmBirthday { get; set; }
        public string? EmBloodGroup { get; set; }
        public string? EmJoiningDate { get; set; }
        public string? EmContactEnd { get; set; }
        public string? EmImage { get; set; }
        public string? EmNid { get; set; }

        public virtual Department? Dep { get; set; }
        public virtual Designation? Des { get; set; }
        public virtual EmpSalary? EmpSalary { get; set; }
        public virtual ICollection<PaySalary> PaySalaries { get; set; } = new List<PaySalary>();
    }
}
