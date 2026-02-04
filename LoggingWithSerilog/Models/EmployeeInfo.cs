

namespace LoggingWithSerilog.Models
{
    public partial class EmployeeInfo
    {
        public int Id { get; set; }

        public string? Fname { get; set; }

        public string? Lname { get; set; }

        public string? Designation { get; set; }

        public int? Salary { get; set; }

        public string? Adress { get; set; }

        public bool Isdeleted { get; set; }
    }
}
