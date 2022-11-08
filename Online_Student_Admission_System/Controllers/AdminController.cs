using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Online_Student_Admission_System.Data;
using Online_Student_Admission_System.Models;
using SelectPdf;
using System.IO;
using System.Net;
using System.Net.Mail;



namespace Online_Student_Admission_System.Controllers
{
    public class AdminController : Controller
    {
      // private readonly IHttpContextAccessor _context;
        private readonly MyDBcontext myDBcontext;
        private readonly IHttpContextAccessor context;

        public AdminController(MyDBcontext myDBcontext, IHttpContextAccessor context)
        {
            this.myDBcontext = myDBcontext;
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ViewAllDepartments()
        {
            var departments = await myDBcontext.Departments.ToListAsync();

            return View(departments);
        }
        

        [HttpGet]
        public IActionResult AddDepartment()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> AddDepartment(AddDepartment ad)
        {
            var department = new Department()
            {   Id = ad.Id,
                Name = ad.Name,
                No_of_slots = ad.No_of_slots
             };

            await myDBcontext.Departments.AddAsync(department);
            await myDBcontext.SaveChangesAsync();
            return RedirectToAction("Index");

        }


        [HttpGet]

        public async Task<IActionResult> UpdateDepartment(int id)
        {
            var department = await myDBcontext.Departments.FirstOrDefaultAsync(x => x.Id == id);
            if(department!=null)
            {
                var viewmodel = new UpdateDepartmentmodel()
                {
                    Id = department.Id,
                    Name = department.Name,
                    No_of_slots = department.No_of_slots

                };

                return View(viewmodel);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]

        public async Task<IActionResult> UpdateDepartment(UpdateDepartmentmodel model)
        {
            var department = await myDBcontext.Departments.FindAsync(model.Id);
            if(department!=null)
            {
                
                department.Name = model.Name;
                department.No_of_slots = model.No_of_slots;

                await myDBcontext.SaveChangesAsync();
                return RedirectToAction("ViewAllDepartments");
            }

            return RedirectToAction("ViewAllDepartments");
        }

        [HttpPost]

        public async Task<IActionResult> DeleteDepartment(UpdateDepartmentmodel model)
        {
            var department = await myDBcontext.Departments.FindAsync(model.Id);
            if(department!=null)
            {
                myDBcontext.Departments.Remove(department);
                await myDBcontext.SaveChangesAsync();

                return RedirectToAction("ViewAllDepartments");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public  IActionResult AddStudent()
        {
            ViewData["DepartmentId"] = new SelectList(myDBcontext.Departments,"Id","Name");

            return View();
        }

        [HttpPost]

        public async Task<IActionResult> AddStudent(AddStudent model)
        {
            ViewData["DepartmentId"] = new SelectList(myDBcontext.Departments, "Id", "Name", model.DepartmentId);
           
            var department =  myDBcontext.Departments.FirstOrDefault(x => x.Id == model.DepartmentId);
            if(department!=null)
            {
                ViewBag.departmentname = department.Name;
               
                await myDBcontext.SaveChangesAsync();

            }


            var student = new Student()
            {
                Id = model.Id,
                Name = model.Name,
                Emailid = model.Emailid,
                Password = model.Password,
                Dob = model.Dob,
                Fee_Status = "pending",
                DepartmentId = model.DepartmentId,
                Is_Approved = false,
                Fee_amount = 0,
                DepartmentName = ViewBag.departmentname,
               
            };

           
            


         /*   var department = await myDBcontext.Departments.FindAsync(ViewData["DepartmentId"]);
            if(department!=null)
            {
                model1.No_of_slots = department.No_of_slots--;
                department.No_of_slots = model1.No_of_slots;
                await myDBcontext.SaveChangesAsync();
            } */

            await myDBcontext.Students.AddAsync(student);
            await myDBcontext.SaveChangesAsync();

            return RedirectToAction("Index");


        }

        [HttpGet]

        public async Task<IActionResult> ViewAllStudents()
        {
            var departments = await myDBcontext.Students.ToListAsync();

            return View(departments);
        }

        [HttpGet]

        public async Task<IActionResult> UpdateStudent(int id)
        {

            var student = await myDBcontext.Students.FirstOrDefaultAsync(x => x.Id == id);
            if (student != null)
            {
                ViewData["DepartmentId"] = new SelectList(myDBcontext.Departments, "Id", "Name");

                var viewmodel = new UpdateStudentmodel()
                {
                    Id = student.Id,
                    Name = student.Name,
                    Emailid = student.Emailid,
                    Password = student.Password,
                    Dob = student.Dob,
                    Fee_Status = student.Fee_Status,
                    DepartmentId = student.DepartmentId,
                    DepartmentName = student.DepartmentName,
                    Is_Approved = student.Is_Approved,
                    Fee_amount = student.Fee_amount,
                   


                };

                // return View(viewmodel);
                return View(viewmodel);
            }

            //  return RedirectToAction("Index");
            return RedirectToAction("Index");
        }

        [HttpPost]

        public async Task<IActionResult> UpdateStudent(UpdateStudentmodel model,Emailmodel model1)
        {
            ViewData["DepartmentId"] = new SelectList(myDBcontext.Departments, "Id", "Name", model.DepartmentId);

            var student = await myDBcontext.Students.FindAsync(model.Id);
            
            if (student != null)
            {
                var Is_Approve = student.Is_Approved;
                student.Name = model.Name;
                student.Emailid = model.Emailid;
                student.Password = model.Password;
                student.Dob = model.Dob;
                student.Fee_Status = model.Fee_Status;
                student.Fee_amount = model.Fee_amount;
                student.Is_Approved = model.Is_Approved;
                student.DepartmentId = model.DepartmentId;
               
                var department = myDBcontext.Departments.FirstOrDefault(x => x.Id == model.DepartmentId);
                if (department != null)
                {
                    student.DepartmentName = department.Name;
                    await myDBcontext.SaveChangesAsync();
                }

               
                await myDBcontext.SaveChangesAsync();

                if (Is_Approve != model.Is_Approved)
                {
                    model1.From = "gautamrathod70502@gmail.com";
                    model1.Password = "zbimsnlpijpvidxt";
                    using (MailMessage mm = new MailMessage(model1.From, model.Emailid))
                    {
                        mm.Subject = "Congratulation, "+ model.Name + " you have been approved by the administration!!!";
                        mm.Body = "Welcome, "+ model.Name + ".Now You can Login to the Website.";
                        mm.IsBodyHtml = false;
                        using(SmtpClient smtp = new SmtpClient())
                        {
                            smtp.Host = "smtp.gmail.com";
                            smtp.EnableSsl = true;
                            NetworkCredential networkCredential = new NetworkCredential(model1.From,model1.Password);
                            smtp.Credentials = networkCredential;
                            smtp.UseDefaultCredentials = false;
                            smtp.Port = 587;
                            smtp.Send(mm);
                        }
                    }
                    
                }
                //return RedirectToAction("ViewAllDepartments");
                return RedirectToAction("ViewAllStudents");
            }


           

            //return RedirectToAction("ViewAllDepartments");
            return RedirectToAction("ViewAllStudents");
        }


        [HttpPost]

        public async Task<IActionResult> DeleteStudent(UpdateStudentmodel model)
        {
            var student =await myDBcontext.Students.FindAsync(model.Id);
            if(student!=null)
            {
                 myDBcontext.Students.Remove(student);
               await myDBcontext.SaveChangesAsync();

                return RedirectToAction("ViewAllStudents");
            }

            return RedirectToAction("ViewAllStudents");
        }


        [HttpGet]

        public  IActionResult Login()
        {
            return View();
        }

        [HttpGet]

        public IActionResult StudentPanel()
        {
            return RedirectToAction("StudentPanel");
        }

        [HttpGet]

        public IActionResult WaitingPage()
        {
            return View();
        }
       

        [HttpPost]

        public async Task<IActionResult> Login(Loginmodel model)
        {
            if(model.Emailid == "admin@gmail.com" && model.Password == "12345")
            {
                return RedirectToAction("Index");
            }

            else if(model.Emailid!= "admin@gmail.com" &&  model.Password!="12345")
            {
                ViewData["DepartmentId"] = new SelectList(myDBcontext.Departments, "Id", "Name", model.DepartmentId);
                var student = await myDBcontext.Students.FirstOrDefaultAsync(x => x.Emailid == model.Emailid && x.Password == model.Password);
                 if(student!=null)
                {
                    //ViewBag.studentinfo = student;

                    // HttpContext.Session.SetInt32("Fees_amount", student.Fee_amount);
                    // HttpContext.Session.SetInt32("Id", student.Id);
                    context.HttpContext?.Session.SetInt32("Id", student.Id);
                    ViewData["Name"] = student.Name;
                    ViewData["Emailid"] = student.Emailid;
                    // ViewData["Password"] = student.Password;
                    ViewData["Password"] = student.Password;
                    ViewData["Dob"] = student.Dob;
                    ViewData["DepartmentName"] = student.DepartmentName;
                    ViewData["Fees_status"] = student.Fee_Status;
                    TempData["Name"] = student.Name;
                    TempData["Fees_status"] = student.Fee_Status;
                    TempData["Fees_amount"] = student.Fee_amount;
                    TempData["Id"] = student.Id;
                    //Session["Id"] = student.Id;

                    if(student.Is_Approved == true)
                    {
                        return View("StudentPanel");
                    }
                        // return RedirectToAction("StudentPanel");
                       else
                    {
                        return RedirectToAction("WaitingPage");
                    }
                    
                  
                }
                 else
                {
                    ViewBag.errormessage = "wrong emaild and password";
                    
                }
            }

            return RedirectToAction("Login");

            
        }

        [HttpGet]
        public async Task<IActionResult> ApprovedStudent()
        {
            var students = await myDBcontext.Students.ToListAsync();

            return View(students);
        }

        [HttpGet]

        public IActionResult Register()
        {
            ViewData["DepartmentId"] = new SelectList(myDBcontext.Departments, "Id", "Name");

            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Register(Loginmodel model)
        {
            ViewData["DepartmentId"] = new SelectList(myDBcontext.Departments, "Id", "Name", model.DepartmentId);

            var department = myDBcontext.Departments.FirstOrDefault(x => x.Id == model.DepartmentId);
            if (department != null)
            {
                ViewBag.departmentname = department.Name;

                await myDBcontext.SaveChangesAsync();

            }


            var student = new Student()
            {
                Id = model.Id,
                Name = model.Name,
                Emailid = model.Emailid,
                Password = model.Password,
                // Dob = model.Dob,
                Dob = model.Dob,
                Fee_Status = "pending",
                DepartmentId = model.DepartmentId,
                Is_Approved = false,
                Fee_amount = 0,
                DepartmentName = ViewBag.departmentname,

            };


            await myDBcontext.Students.AddAsync(student);
            await myDBcontext.SaveChangesAsync();

            return RedirectToAction("Register");
        }


        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }

        public  IActionResult PayFees()
        {
           
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> PayFees(Loginmodel model)
        {
            var id = context.HttpContext?.Session.GetInt32("Id");
            var student = await myDBcontext.Students.FirstOrDefaultAsync(x => x.Id == id);
            if(student!=null)
            {
                ViewData["fee_status"] = student.Fee_Status;
                ViewData["fee_amount"] = student.Fee_amount;
                if(student.Fee_Status == "pending")
                {
                    if(student.Fee_amount == model.Fee_amount)
                    {
                        student.Fee_Status = "paid";
                       await myDBcontext.SaveChangesAsync();
                    }
                }
                return RedirectToAction("Login");
            }

            return RedirectToAction("Login");
        }

        
      /*  public IActionResult GeneratePdf(string html)
        {
            html = html.Replace("StrTag", "<").Replace("EndTag", ">");
            HtmlToPdf oHtmlToPdf = new HtmlToPdf();
            PdfDocument oPdfDocument = oHtmlToPdf.ConvertHtmlString(html);
            byte[] pdf = oPdfDocument.Save();
            oPdfDocument.Close();
            return File(
                pdf,
                "application/pdf",
                "Admissionform.pdf"
                );


        }*/

            
        










    }
}
