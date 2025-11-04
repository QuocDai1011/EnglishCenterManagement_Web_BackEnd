using EnglishCenterManagement_BackEnd.Models;
using EnglishCenterManagement_BackEnd.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishCenterManagement_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly EnglishCenterManagementDevContext _context;

        public ClassController(EnglishCenterManagementDevContext context)
        {
            _context = context;
        }

        // [GET] /api/class
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var classes = await _context.Classes.ToListAsync();
            classes = ClassService.AutoSetStatus(classes);
            await _context.SaveChangesAsync();
            return Ok(classes);
        }

        // [GET] /api/class/get-courses-by-id/id
        [HttpGet("get-courses-by-id/{id}")]
        public async Task<IActionResult> GetCoursesById(int id)
        {
            var classWithCourses = await _context.Classes
               .Include(c => c.Courses)
               .FirstOrDefaultAsync(c => c.ClassId == id);

            if (classWithCourses == null)
                return NotFound("Không tìm thấy lớp học!");

            return Ok(classWithCourses.Courses);
        }

        // [GET] /api/class/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var _class = await _context.Classes.FindAsync(id);
            return Ok(_class);
        }

        // [POST] /api/class
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Class newClass)
        {
            if (newClass == null) { 
                return BadRequest("Class' data is required!");
            }

            try
            {
                _context.Classes.Add(newClass);

                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new {id = newClass.ClassId}, newClass);
            }
            catch (Exception ex) {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // [DELETE] /api/class/id
        [HttpDelete("{id}")] 
        public async Task<IActionResult> Delete(int id)
        {
            var _class = await _context.Classes.FindAsync(id);
            if(_class == null) return NotFound("Không tìm thấy dữ liệu lớp học!");

            try
            {
                _context.Classes.Remove(_class);
                await _context.SaveChangesAsync();
                return Ok("Xóa lớp học thành công");
            }
            catch (Exception ex) {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        //[PUT] /api/class/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Class updatedClass)
        {
            var _class = await _context.Classes.FindAsync(id);

            if (_class == null) return NotFound("Không tìm thấy dữ liệu lớp học!");

            try
            {
                _class = ClassService.Mapper(_class, updatedClass);
                await _context.SaveChangesAsync();
                return Ok("Cập nhật lớp học thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật khóa học thất bại: {ex.Message}");
            }
        }

        // [GET] /api/Class/get-classes/{id}
        [HttpGet("get-students/{id}")]
        public async Task<IActionResult> GetStudents (int id)
        {
            var studentClasses = await _context.Classes
                .Include(c => c.Students)
                .Where(c => c.ClassId == id)
                .Select( c => new
                {
                    c.ClassId,
                    c.ClassCode,
                    c.ClassName,
                    c.MaxStudent,
                    c.CurrentStudent,
                    c.StartDate,
                    c.EndDate,
                    c.Status,
                    c.OnlineMeetingLink,
                    Students = c.Students
                }
                )
                .ToListAsync();
            if (studentClasses == null) return NotFound("Không tìm thấy dữ liệu!");

            return Ok(studentClasses);
        }

        // [GET] /api/class/get-courses/{id}
        [HttpGet("get-courses/{id}")]
        public async Task<IActionResult> GetCourses (int id)
        {
            var classCourses = await _context.Classes
                .Include(c => c.Courses)
                .Where(c => c.ClassId == id)
                .Select(c => new
                {
                    c.ClassId,
                    c.ClassCode,
                    c.ClassName,
                    c.MaxStudent,
                    c.CurrentStudent,
                    c.StartDate,
                    c.EndDate,
                    c.Shift,
                    c.Status,
                    c.Note,
                    c.CreateAt,
                    c.OnlineMeetingLink,
                    Courses = c.Courses
                })
                .ToListAsync();

            if (classCourses == null) return NotFound(new {message = "Không tìm thấy dữ liệu."});
            return Ok(classCourses);
        }

        // [GET] /api/class/get-teachers/{id}
        [HttpGet("get-teachers/{id}")]
        public async Task<IActionResult> GetTeachers(int id)
        {
            var classTeacher = await _context.Classes
                .Include(c => c.Teachers)
                .Select(c => new 
                {
                    c.ClassId,
                    c.ClassCode,
                    c.ClassName,
                    c.MaxStudent,
                    c.CurrentStudent,
                    c.StartDate,
                    c.EndDate,
                    c.Shift,
                    c.Status,
                    c.Note,
                    c.CreateAt,
                    c.OnlineMeetingLink,
                    Teachers = c.Teachers
                })
                .ToListAsync();

            if (classTeacher == null) return NotFound(new { message = "Không tìm thấy dữ liệu." });
            return Ok(classTeacher);
        }
        
    }
}
