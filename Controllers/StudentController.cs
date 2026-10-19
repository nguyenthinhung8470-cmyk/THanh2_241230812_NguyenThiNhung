using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using THanh2.Models;

namespace THanh2.Controllers
{
    [Route("Admin/Student")]
    public class StudentController : Controller
    {
        // Dùng static để lưu dữ liệu tạm thời khi thêm mới mà không bị reset
        private static List<Student> listStudents = new List<Student>();
        private readonly IWebHostEnvironment _env;

        // Inject IWebHostEnvironment qua Constructor
        public StudentController(IWebHostEnvironment env)
        {
            _env = env;

            // Khởi tạo 4 sinh viên mẫu nếu danh sách đang rỗng
            if (!listStudents.Any())
            {
                listStudents = new List<Student>()
                {
                    new Student() { Id = 101, Name = "Hải Nam", Branch = Branch.IT, Gender = Gender.Male, IsRegular = true, Address = "A1-2018", Email = "nam@g.com" },
                    new Student() { Id = 102, Name = "Minh Tú", Branch = Branch.BE, Gender = Gender.Female, IsRegular = true, Address = "A1-2019", Email = "tu@g.com" },
                    new Student() { Id = 103, Name = "Hoàng Phong", Branch = Branch.CE, Gender = Gender.Male, IsRegular = false, Address = "A1-2019", Email = "phong@g.com" },
                    new Student() { Id = 104, Name = "Xuân Mai", Branch = Branch.EE, Gender = Gender.Female, IsRegular = false, Address = "A1-2021", Email = "mai@g.com" }
                };
            }
        }

        [HttpGet("List", Name = "student_list")]
        [Route("")]
        public IActionResult Index()
        {
            return View(listStudents);
        }

        [HttpGet("Add", Name = "student_add")]
        public IActionResult Create()
        {
            ViewBag.AllGenders = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();
            ViewBag.AllBranches = new List<SelectListItem>()
            {
                new SelectListItem { Text = "IT", Value = "1" },
                new SelectListItem { Text = "BE", Value = "2" },
                new SelectListItem { Text = "CE", Value = "3" },
                new SelectListItem { Text = "EE", Value = "4" }
            };
            return View();
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Create(Student s)
        {
            // Xử lý upload ảnh nếu người dùng có chọn file
            if (s.AvatarFile != null && s.AvatarFile.Length > 0)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "images");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(s.AvatarFile.FileName);
                string filePath = Path.Combine(uploadDir, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await s.AvatarFile.CopyToAsync(fileStream);
                }

                s.AvatarUrl = "/images/" + fileName;
            }

            s.Id = listStudents.Any() ? listStudents.Last().Id + 1 : 101;
            listStudents.Add(s);

            return View("Index", listStudents);
        }
    }
}