using EnglishCenterManagement_BackEnd.Models;
using GenerativeAI.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public async Task<IActionResult> GetByIdOfStudent(int id)
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
            if (studentCourse == null)
            {
                return NotFound(new { message = "Không tìm thấy dữ liệu." });
            }
            return Ok(studentCourse);
        }

        //[POST] /api/studentcourse
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StudentCourse studentCourse)
        {
            if (studentCourse == null)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            var exist = await _context.StudentCourses.FindAsync(
                   studentCourse.StudentId,
                   studentCourse.CourseId
            );

            


            if (exist != null)
            {
                return Conflict(new { message = "Học viên đã đăng ký khóa học này rồi, vui lòng sử dụng chức năng cập nhật." });
            }

            try
            {
                studentCourse.CreateAt = DateTime.Now;
                studentCourse.UpdateAt = null;
                _context.StudentCourses.Add(studentCourse);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Thêm StudentCourse thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Lỗi server.",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        // [PUT] /api/studentcourse/{id}
        [HttpPut("{studentId}")]
        public async Task<IActionResult> UpdateByIdOfStudent(int studentId, [FromBody] StudentCourse newStudentCourse)
        {
            var studentCourse = await _context.StudentCourses.FindAsync(studentId);

            if (studentCourse == null)
            {
                return Conflict(new { message = "Không tìm thấy dữ liệu của học viên." });
            }

            if (studentCourse.CourseId == newStudentCourse.CourseId)
            {
                try
                {
                    studentCourse.UpdateAt = DateTime.Now;
                    studentCourse.DiscountType = newStudentCourse.DiscountType;
                    studentCourse.DiscountValue = newStudentCourse.DiscountValue;
                    _context.StudentCourses.Update(studentCourse);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Cập nhật khóa học thành công." });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Error: " + ex.Message);
                }
            }
            else
            {
                return Conflict(new { message = "Dữ liệu khoog thể cập nhật vì học viên chưa từng học khóa học này." });
            }
        }
    }
}
