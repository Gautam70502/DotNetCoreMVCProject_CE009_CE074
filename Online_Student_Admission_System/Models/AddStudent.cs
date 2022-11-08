using System.ComponentModel.DataAnnotations;

namespace Online_Student_Admission_System.Models
{
    public class AddStudent
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Emailid { get; set; }

        [Required]

        public string Password { get; set; }

        [Required]

        public DateTime Dob { get; set; }

        public int DepartmentId { get; set; }

        public string? DepartmentName { get; set; }
        public Department Department { get; set; }
    }
}
