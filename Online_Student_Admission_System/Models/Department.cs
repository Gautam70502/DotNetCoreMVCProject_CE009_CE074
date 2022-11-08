using System.ComponentModel.DataAnnotations;

namespace Online_Student_Admission_System.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int No_of_slots { get; set; }

        public ICollection<Student> Students { get; set; }
    }
}

