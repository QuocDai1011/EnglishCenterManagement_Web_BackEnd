using EnglishCenterManagement_BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishCenterManagement_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {

        private readonly EnglishCenterManagementDevContext _context;

        public TeacherController(EnglishCenterManagementDevContext context)
        {
            _context = context;
        }

        // [GET] /api/teacher
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teachers = await _context.Teachers.ToListAsync();
            return Ok(teachers);
        }

        // [GET] /api/teacher/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound("Không tìm thấy giảng viên có id là " + id);
            }
            return Ok(teacher);
        }

        // [POST] /api/teacher
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Teacher teacher)
        {
            if (teacher == null)
            {
                return BadRequest("Thông tin giảng viên không được trống!");
            }

            var exist = await _context.Teachers.AnyAsync(t => t.UserName == teacher.UserName);

            if (exist)
            {
                return Conflict("Username đã tồn tại, vui lòng chọn tên khác.");
            }
            else
            {
                try
                {
                    await _context.Teachers.AddAsync(teacher);

                    await _context.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetById), new { id = teacher.AdminId }, teacher);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }

        // [DELETE] /api/teacher/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete (int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if(teacher == null)
            {
                return NotFound(new {message = "Không tìm thấy thông tin giảng viên."});
            }

            try
            {
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
                return Ok("Xóa giảng viên thành công.");
            }catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
            
        }

        // [PUT] /api/teacher/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update (int id, [FromBody] Teacher newTeacher)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if(teacher == null)
            {
                return NotFound("Không tìm thấy thông tin giảng viên.");
            }

            return Ok();
        }


    }
}
