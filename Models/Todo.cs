using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_CSharp.Models
{
    [Table("to-do_list")]
    public class Todo
    {
        [Key]
        public int Id { get; set; }

        [Column("user_id")]
        public string? UserId { get; set; }

        [Column("to_dodata")]
        public string? ToDoData { get; set; }

        public string? Value { get; set; }

        public string? Date { get; set; }
    }
}
