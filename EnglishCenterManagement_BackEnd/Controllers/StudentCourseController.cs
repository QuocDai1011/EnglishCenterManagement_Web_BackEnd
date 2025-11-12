using EnglishCenterManagement_BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishCenterManagement_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentCourseController : ControllerBase
    {
        private readonly EnglishCenterManagementDevContext _context;

        public StudentCourseController(EnglishCenterManagementDevContext context)
        {
            _context = context;
        }

        //[GET] /api/studentcourse
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var studentCourses = await _context.StudentCourses
                .Include(c => c.Course)
                .Include(c => c.Student)
                .Select(c => new
                    {
                        c.StudentId,
                        c.CourseId,
                        c.DiscountType,
                        c.DiscountValue,
                        c.CreateAt,
                        Course = new
                        {
                            c.Course.CourseName,
                            c.Course.Level,
                            c.Course.TutitionFee
                        },
                        Student = new
                        {
                            c.Student.FullName,
                            c.Student.Email
                        }
                }
                )
                .ToListAsync();
            return Ok(studentCourses);
        }

        // [GET] /api/studentcourse/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdOfStudent (int id)
        {
            var studentCourse = await _context.StudentCourses
                .Include(c => c.Course)
                .Include(c => c.Student)
                .Where(c => c.StudentId == id)
                .Select(c => new
                {
                    c.StudentId,
                    c.CourseId,
                    c.DiscountType,
                    c.DiscountValue,
                    c.CreateAt,
                    Student = new
                    {
                        c.Student.FullName,
                        c.Student.Email
                    },
                    Course = new
                    {
                        c.Course.CourseName,
                        c.Course.Level,
                        c.Course.TutitionFee
                    }
                })
                .ToListAsync();
            if(studentCourse == null)
            {
                return NotFound(new {message = "Không tìm thấy dữ liệu."});
            }
            return Ok(studentCourse);
        }

        //[POST] /api/studentcourse/get-bill
        //[HttpPost("get-bill")]
        //public async Task<IActionResult> GetBill([FromBody] StudentCourse studentCourse)
        //{
            
        //}
    }
}
