using Microsoft.EntityFrameworkCore;
using Online_Student_Admission_System.Models;

namespace Online_Student_Admission_System.Data
{
    public class MyDBcontext : DbContext
    {
        public MyDBcontext(DbContextOptions<MyDBcontext> options) : base(options)
        {
                
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Student> Students { get; set; }
    }
}
