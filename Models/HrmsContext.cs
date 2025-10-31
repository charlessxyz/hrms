using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HRMS_CSharp.Models
{
    public partial class HrmsContext : DbContext
    {
        public HrmsContext()
        {
        }

        public HrmsContext(DbContextOptions<HrmsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Addition> Additions { get; set; } = null!;
        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<Asset> Assets { get; set; } = null!;
        public virtual DbSet<AssetsCategory> AssetsCategories { get; set; } = null!;
        public virtual DbSet<AssignLeave> AssignLeaves { get; set; } = null!;
        public virtual DbSet<AssignTask> AssignTasks { get; set; } = null!;
        public virtual DbSet<Attendance> Attendances { get; set; } = null!;
        public virtual DbSet<BankInfo> BankInfos { get; set; } = null!;
        public virtual DbSet<Deduction> Deductions { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Desciplinary> Desciplinaries { get; set; } = null!;
        public virtual DbSet<Designation> Designations { get; set; } = null!;
        public virtual DbSet<EarnedLeave> EarnedLeaves { get; set; } = null!;
        public virtual DbSet<Education> Educations { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<EmployeeFile> EmployeeFiles { get; set; } = null!;
        public virtual DbSet<EmpAsset> EmpAssets { get; set; } = null!;
        public virtual DbSet<EmpExperience> EmpExperiences { get; set; } = null!;
        public virtual DbSet<EmpLeave> EmpLeaves { get; set; } = null!;
        public virtual DbSet<EmpPenalty> EmpPenalties { get; set; } = null!;
        public virtual DbSet<EmpSalary> EmpSalaries { get; set; } = null!;
        public virtual DbSet<EmpTraining> EmpTrainings { get; set; } = null!;
        public virtual DbSet<FieldVisit> FieldVisits { get; set; } = null!;
        public virtual DbSet<Holiday> Holidays { get; set; } = null!;
        public virtual DbSet<LeaveType> LeaveTypes { get; set; } = null!;
        public virtual DbSet<Loan> Loans { get; set; } = null!;
        public virtual DbSet<LoanInstallment> LoanInstallments { get; set; } = null!;
        public virtual DbSet<Notice> Notices { get; set; } = null!;
        public virtual DbSet<PaySalary> PaySalaries { get; set; } = null!;
        public virtual DbSet<Penalty> Penalties { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<ProjectTask> ProjectTasks { get; set; } = null!;
        public virtual DbSet<ProjectFile> ProjectFiles { get; set; } = null!;
        public virtual DbSet<ProjectNote> ProjectNotes { get; set; } = null!;
        public virtual DbSet<ProjectExpense> ProjectExpenses { get; set; } = null!;
        public virtual DbSet<SalaryType> SalaryTypes { get; set; } = null!;
        public virtual DbSet<Settings> Settings { get; set; } = null!;
        public virtual DbSet<SocialMedium> SocialMedia { get; set; } = null!;
        public virtual DbSet<Todo> Todos { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost;Database=hrsystemci;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Addition>(entity =>
            {
                entity.HasKey(e => e.AddiId);

                entity.ToTable("addition");

                entity.Property(e => e.AddiId).HasColumnName("addi_id");
                entity.Property(e => e.Basic).HasColumnName("basic");
                entity.Property(e => e.Conveyance).HasColumnName("conveyance");
                entity.Property(e => e.HouseRent).HasColumnName("house_rent");
                entity.Property(e => e.Medical).HasColumnName("medical");
                entity.Property(e => e.SalaryId).HasColumnName("salary_id");
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("address");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Address1).HasColumnName("address");
                entity.Property(e => e.City).HasColumnName("city");
                entity.Property(e => e.Country).HasColumnName("country");
                entity.Property(e => e.EmpId).HasColumnName("emp_id");
                entity.Property(e => e.Type).HasColumnName("type");
            });

            modelBuilder.Entity<Asset>(entity =>
            {
                entity.ToTable("assets");

                entity.Property(e => e.AssId).HasColumnName("ass_id");
                entity.Property(e => e.AssBrand).HasColumnName("ass_brand");
                entity.Property(e => e.AssCode).HasColumnName("ass_code");
                entity.Property(e => e.AssModel).HasColumnName("ass_model");
                entity.Property(e => e.AssName).HasColumnName("ass_name");
                entity.Property(e => e.AssPrice).HasColumnName("ass_price");
                entity.Property(e => e.AssQty).HasColumnName("ass_qty");
                entity.Property(e => e.Catid).HasColumnName("catid");
                entity.Property(e => e.Configuration).HasColumnName("configuration");
                entity.Property(e => e.InStock).HasColumnName("in_stock");
                entity.Property(e => e.PurchasingDate).HasColumnName("purchasing_date");
            });

            modelBuilder.Entity<AssetsCategory>(entity =>
            {
                entity.HasKey(e => e.CatId);

                entity.ToTable("assets_category");

                entity.Property(e => e.CatId).HasColumnName("cat_id");
                entity.Property(e => e.CatName).HasColumnName("cat_name");
                entity.Property(e => e.CatStatus).HasColumnName("cat_status");
            });

            modelBuilder.Entity<AssignLeave>(entity =>
            {
                entity.ToTable("assign_leave");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AppId).HasColumnName("app_id");
                entity.Property(e => e.Day).HasColumnName("day");
                entity.Property(e => e.EmpId).HasColumnName("emp_id");
                entity.Property(e => e.Hour).HasColumnName("hour");
                entity.Property(e => e.TotalDay).HasColumnName("total_day");
                entity.Property(e => e.TypeId).HasColumnName("type_id");
                entity.Property(e => e.Dateyear).HasColumnName("dateyear");
            });

            modelBuilder.Entity<AssignTask>(entity =>
            {
                entity.ToTable("assign_task");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AssignUser).HasColumnName("assign_user");
                entity.Property(e => e.ProjectId).HasColumnName("project_id");
                entity.Property(e => e.TaskId).HasColumnName("task_id");
                entity.Property(e => e.UserType).HasColumnName("user_type");
            });

            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.ToTable("attendance");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Absence).HasColumnName("absence");
                entity.Property(e => e.AttenDate).HasColumnName("atten_date");
                entity.Property(e => e.Earnleave).HasColumnName("earnleave");
                entity.Property(e => e.EmpId).HasColumnName("emp_id");
                entity.Property(e => e.Overtime).HasColumnName("overtime");
                entity.Property(e => e.Place).HasColumnName("place");
                entity.Property(e => e.SigninTime).HasColumnName("signin_time");
                entity.Property(e => e.SignoutTime).HasColumnName("signout_time");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.WorkingHour).HasColumnName("working_hour");
            });

            modelBuilder.Entity<BankInfo>(entity =>
            {
                entity.ToTable("bank_info");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AccountNumber).HasColumnName("account_number");
                entity.Property(e => e.AccountType).HasColumnName("account_type");
                entity.Property(e => e.BankName).HasColumnName("bank_name");
                entity.Property(e => e.BranchName).HasColumnName("branch_name");
                entity.Property(e => e.EmId).HasColumnName("em_id");
                entity.Property(e => e.HolderName).HasColumnName("holder_name");
            });

            modelBuilder.Entity<Deduction>(entity =>
            {
                entity.ToTable("deduction");

                entity.Property(e => e.DeId).HasColumnName("de_id");
                entity.Property(e => e.Bima).HasColumnName("bima");
                entity.Property(e => e.Others).HasColumnName("others");
                entity.Property(e => e.ProvidentFund).HasColumnName("provident_fund");
                entity.Property(e => e.SalaryId).HasColumnName("salary_id");
                entity.Property(e => e.Tax).HasColumnName("tax");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("department");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DepName).HasColumnName("dep_name");
            });

            modelBuilder.Entity<Desciplinary>(entity =>
            {
                entity.ToTable("desciplinary");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Action).HasColumnName("action");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.EmId).HasColumnName("em_id");
                entity.Property(e => e.Title).HasColumnName("title");
            });

            modelBuilder.Entity<Designation>(entity =>
            {
                entity.ToTable("designation");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DesName).HasColumnName("des_name");
            });

            modelBuilder.Entity<EarnedLeave>(entity =>
            {
                entity.ToTable("earned_leave");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.EmId).HasColumnName("em_id");
                entity.Property(e => e.Hour).HasColumnName("hour");
                entity.Property(e => e.PresentDate).HasColumnName("present_date");
                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<Education>(entity =>
            {
                entity.ToTable("education");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.EduType).HasColumnName("edu_type");
                entity.Property(e => e.EmpId).HasColumnName("emp_id");
                entity.Property(e => e.Institute).HasColumnName("institute");
                entity.Property(e => e.Result).HasColumnName("result");
                entity.Property(e => e.Year).HasColumnName("year");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("employee");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DepId).HasColumnName("dep_id");
                entity.Property(e => e.DesId).HasColumnName("des_id");
                entity.Property(e => e.EmAddress).HasColumnName("em_address");
                entity.Property(e => e.EmBirthday).HasColumnName("em_birthday");
                entity.Property(e => e.EmBloodGroup).HasColumnName("em_blood_group");
                entity.Property(e => e.EmCode).HasColumnName("em_code");
                entity.Property(e => e.EmContactEnd).HasColumnName("em_contact_end");
                entity.Property(e => e.EmEmail).HasColumnName("em_email");
                entity.Property(e => e.EmGender).HasColumnName("em_gender");
                entity.Property(e => e.EmId).HasColumnName("em_id");
                entity.Property(e => e.EmImage).HasColumnName("em_image");
                entity.Property(e => e.EmJoiningDate).HasColumnName("em_joining_date");
                entity.Property(e => e.EmNid).HasColumnName("em_nid");
                entity.Property(e => e.EmPassword).HasColumnName("em_password");
                entity.Property(e => e.EmPhone).HasColumnName("em_phone");
                entity.Property(e => e.EmRole).HasColumnName("em_role");
                entity.Property(e => e.EmStatus).HasColumnName("em_status");
                entity.Property(e => e.FirstName).HasColumnName("first_name");
                entity.Property(e => e.LastName).HasColumnName("last_name");

                entity.HasOne(d => d.Dep).WithMany(p => p.Employees)
                    .HasForeignKey(d => d.DepId)
                    .HasConstraintName("FK_employee_department");

                entity.HasOne(d => d.Des).WithMany(p => p.Employees)
                    .HasForeignKey(d => d.DesId)
                    .HasConstraintName("FK_employee_designation");
            });

            modelBuilder.Entity<EmployeeFile>(entity =>
            {
                entity.ToTable("employee_file");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.EmId).HasColumnName("em_id");
                entity.Property(e => e.FileTitle).HasColumnName("file_title");
                entity.Property(e => e.FileUrl).HasColumnName("file_url");
            });

            modelBuilder.Entity<EmpAsset>(entity =>
            {
                entity.ToTable("emp_assets");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AssetsId).HasColumnName("assets_id");
                entity.Property(e => e.EmpId).HasColumnName("emp_id");
                entity.Property(e => e.GivenDate).HasColumnName("given_date");
                entity.Property(e => e.ReturnDate).HasColumnName("return_date");
            });

            modelBuilder.Entity<EmpExperience>(entity =>
            {
                entity.ToTable("emp_experience");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.EmpId).HasColumnName("emp_id");
                entity.Property(e => e.ExpComAddress).HasColumnName("exp_com_address");
                entity.Property(e => e.ExpComPosition).HasColumnName("exp_com_position");
                entity.Property(e => e.ExpCompany).HasColumnName("exp_company");
                entity.Property(e => e.ExpWorkduration).HasColumnName("exp_workduration");
            });

            modelBuilder.Entity<EmpLeave>(entity =>
            {
                entity.ToTable("emp_leave");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ApplyDate).HasColumnName("apply_date");
                entity.Property(e => e.EmId).HasColumnName("em_id");
                entity.Property(e => e.EndDate).HasColumnName("end_date");
                entity.Property(e => e.LeaveDuration).HasColumnName("leave_duration");
                entity.Property(e => e.LeaveStatus).HasColumnName("leave_status");
                entity.Property(e => e.LeaveType).HasColumnName("leave_type");
                entity.Property(e => e.Reason).HasColumnName("reason");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
                entity.Property(e => e.Typeid).HasColumnName("typeid");
            });

            modelBuilder.Entity<EmpPenalty>(entity =>
            {
                entity.ToTable("emp_penalty");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.EmpId).HasColumnName("emp_id");
                entity.Property(e => e.PenaltyDesc).HasColumnName("penalty_desc");
                entity.Property(e => e.PenaltyId).HasColumnName("penalty_id");
            });

            modelBuilder.Entity<EmpSalary>(entity =>
            {
                entity.ToTable("emp_salary");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.EmpId).HasColumnName("emp_id");
                entity.Property(e => e.Total).HasColumnName("total");
                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.HasOne(d => d.Emp).WithOne(p => p.EmpSalary)
                    .HasForeignKey<EmpSalary>(d => d.EmpId)
                    .HasPrincipalKey<Employee>(p => p.EmId)
                    .HasConstraintName("FK_emp_salary_employee");

                entity.HasOne(d => d.Type).WithMany(p => p.EmpSalaries)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_emp_salary_salary_type");
            });

            modelBuilder.Entity<EmpTraining>(entity =>
            {
                entity.ToTable("emp_training");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.EmpId).HasColumnName("emp_id");
                entity.Property(e => e.TrainigId).HasColumnName("trainig_id");
            });

            modelBuilder.Entity<FieldVisit>(entity =>
            {
                entity.ToTable("field_visit");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ActualReturnDate).HasColumnName("actual_return_date");
                entity.Property(e => e.ApproxEndDate).HasColumnName("approx_end_date");
                entity.Property(e => e.AttendanceUpdated).HasColumnName("attendance_updated");
                entity.Property(e => e.EmpId).HasColumnName("emp_id");
                entity.Property(e => e.FieldLocation).HasColumnName("field_location");
                entity.Property(e => e.Notes).HasColumnName("notes");
                entity.Property(e => e.ProjectId).HasColumnName("project_id");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.TotalDays).HasColumnName("total_days");
            });

            modelBuilder.Entity<Holiday>(entity =>
            {
                entity.ToTable("holiday");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.FromDate).HasColumnName("from_date");
                entity.Property(e => e.HolidayName).HasColumnName("holiday_name");
                entity.Property(e => e.NumberOfDays).HasColumnName("number_of_days");
                entity.Property(e => e.ToDate).HasColumnName("to_date");
                entity.Property(e => e.Year).HasColumnName("year");
            });

            modelBuilder.Entity<LeaveType>(entity =>
            {
                entity.HasKey(e => e.TypeId);

                entity.ToTable("leave_types");

                entity.Property(e => e.TypeId).HasColumnName("type_id");
                entity.Property(e => e.LeaveDay).HasColumnName("leave_day");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.ToTable("loan");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Amount).HasColumnName("amount");
                entity.Property(e => e.ApproveDate).HasColumnName("approve_date");
                entity.Property(e => e.EmpId).HasColumnName("emp_id");
                entity.Property(e => e.InstallPeriod).HasColumnName("install_period");
                entity.Property(e => e.Installment).HasColumnName("installment");
                entity.Property(e => e.InterestPercentage).HasColumnName("interest_percentage");
                entity.Property(e => e.LoanDetails).HasColumnName("loan_details");
                entity.Property(e => e.LoanNumber).HasColumnName("loan_number");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.TotalAmount).HasColumnName("total_amount");
                entity.Property(e => e.TotalDue).HasColumnName("total_due");
                entity.Property(e => e.TotalPay).HasColumnName("total_pay");
            });

            modelBuilder.Entity<LoanInstallment>(entity =>
            {
                entity.ToTable("loan_installment");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AppDate).HasColumnName("app_date");
                entity.Property(e => e.EmpId).HasColumnName("emp_id");
                entity.Property(e => e.InstallAmount).HasColumnName("install_amount");
                entity.Property(e => e.InstallNo).HasColumnName("install_no");
                entity.Property(e => e.LoanId).HasColumnName("loan_id");
                entity.Property(e => e.LoanNumber).HasColumnName("loan_number");
                entity.Property(e => e.Notes).HasColumnName("notes");
                entity.Property(e => e.PayAmount).HasColumnName("pay_amount");
                entity.Property(e => e.Receiver).HasColumnName("receiver");
            });

            modelBuilder.Entity<Notice>(entity =>
            {
                entity.ToTable("notice");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Date).HasColumnName("date");
                entity.Property(e => e.FileUrl).HasColumnName("file_url");
                entity.Property(e => e.Title).HasColumnName("title");
            });

            modelBuilder.Entity<PaySalary>(entity =>
            {
                entity.ToTable("pay_salary");

                entity.Property(e => e.PayId).HasColumnName("pay_id");
                entity.Property(e => e.Addition).HasColumnName("addition");
                entity.Property(e => e.Basic).HasColumnName("basic");
                entity.Property(e => e.Bima).HasColumnName("bima");
                entity.Property(e => e.Bonus).HasColumnName("bonus");
                entity.Property(e => e.Diduction).HasColumnName("diduction");
                entity.Property(e => e.EmpId).HasColumnName("emp_id");
                entity.Property(e => e.HouseRent).HasColumnName("house_rent");
                entity.Property(e => e.Loan).HasColumnName("loan");
                entity.Property(e => e.Medical).HasColumnName("medical");
                entity.Property(e => e.Month).HasColumnName("month");
                entity.Property(e => e.PaidDate).HasColumnName("paid_date");
                entity.Property(e => e.PaidType).HasColumnName("paid_type");
                entity.Property(e => e.ProvidentFund).HasColumnName("provident_fund");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.Tax).HasColumnName("tax");
                entity.Property(e => e.TotalDays).HasColumnName("total_days");
                entity.Property(e => e.TotalPay).HasColumnName("total_pay");
                entity.Property(e => e.TypeId).HasColumnName("type_id");
                entity.Property(e => e.Year).HasColumnName("year");

                entity.HasOne(d => d.Emp).WithMany(p => p.PaySalaries)
                    .HasForeignKey(d => d.EmpId)
                    .HasPrincipalKey(p => p.EmId)
                    .HasConstraintName("FK_pay_salary_employee");

                entity.HasOne(d => d.Type).WithMany(p => p.PaySalaries)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_pay_salary_salary_type");
            });

            modelBuilder.Entity<Penalty>(entity =>
            {
                entity.ToTable("penalty");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.PenaltyName).HasColumnName("penalty_name");
                entity.Property(e => e.PenaltyAmount).HasColumnName("penalty_amount");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("project");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ProEndDate).HasColumnName("pro_end_date");
                entity.Property(e => e.ProName).HasColumnName("pro_name");
                entity.Property(e => e.ProStartDate).HasColumnName("pro_start_date");
                entity.Property(e => e.ProStatus).HasColumnName("pro_status");
            });

            modelBuilder.Entity<ProjectTask>(entity =>
            {
                entity.ToTable("project_task");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ProjectId).HasColumnName("project_id");
                entity.Property(e => e.TaskTitle).HasColumnName("task_title");
                entity.Property(e => e.TaskStartDate).HasColumnName("task_start_date");
                entity.Property(e => e.TaskEndDate).HasColumnName("task_end_date");
                entity.Property(e => e.TaskStatus).HasColumnName("task_status");
                entity.Property(e => e.TaskProgress).HasColumnName("task_progress");
            });

            modelBuilder.Entity<SalaryType>(entity =>
            {
                entity.ToTable("salary_type");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.SalaryType1).HasColumnName("salary_type");
            });

            modelBuilder.Entity<Settings>(entity =>
            {
                entity.ToTable("settings");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Skey).HasColumnName("skey");
                entity.Property(e => e.Svalue).HasColumnName("svalue");
            });

            modelBuilder.Entity<SocialMedium>(entity =>
            {
                entity.ToTable("social_media");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.EmpId).HasColumnName("emp_id");
                entity.Property(e => e.Facebook).HasColumnName("facebook");
                entity.Property(e => e.GooglePlus).HasColumnName("google_plus");
                entity.Property(e => e.SkypeId).HasColumnName("skype_id");
                entity.Property(e => e.Twitter).HasColumnName("twitter");
            });

            modelBuilder.Entity<Todo>(entity =>
            {
                entity.ToTable("to-do_list");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.ToDoData).HasColumnName("to_dodata");
                entity.Property(e => e.Value).HasColumnName("value");
                entity.Property(e => e.Date).HasColumnName("date");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
