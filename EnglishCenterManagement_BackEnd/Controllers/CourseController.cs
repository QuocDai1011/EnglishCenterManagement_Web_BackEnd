using EnglishCenterManagement_BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnglishCenterManagement_BackEnd.Service;
using EnglishCenterManagement_BackEnd.Utils;

namespace EnglishCenterManagement_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly EnglishCenterManagementDevContext _context;

        public CourseController(EnglishCenterManagementDevContext context) {
            _context = context;
        }

        // [GET] /api/course
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _context.Courses.ToListAsync();
            return Ok(courses);
        }

        // [GET] /api/course/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null) return NotFound(ErrorEnums.NOT_FOUND_WITH_MODEL("Khóa học"));

            return Ok(course);
        }

        // [POST] /api/course
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Course newCourse)
        {
            if (newCourse == null)
            {
                return BadRequest(ErrorEnums.LACK_OF_FIELD);
            }

            try
            {
                await _context.Courses.AddAsync(newCourse);

                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = newCourse.CourseId }, newCourse);
            }
            catch (Exception ex) {
                return Conflict(ErrorEnums.SERVER_ERROR);
            }
        }

        // [DELETE] /api/course/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null) return NotFound(ErrorEnums.NOT_FOUND_WITH_MODEL("Khóa học"));

            try
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                return Ok("Xóa khóa học thành công!");
            }
            catch (Exception ex)
            {
                return Conflict(ErrorEnums.SERVER_ERROR);
            }
        }

        // [PUT] /api/course/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Course updatedCourse)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound(ErrorEnums.NOT_FOUND_WITH_MODEL("Lớp học"));

            try
            {
                course = CourseService.Mapper(course, updatedCourse);

                _context.Courses.Update(course);

                await _context.SaveChangesAsync();
                return Ok("Cập nhật khóa học thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật khóa học thất bại: {ex.Message}");
            }
        }
    }
}
