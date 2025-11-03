using EnglishCenterManagement_BackEnd.Models;
using EnglishCenterManagement_BackEnd.Service;
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

            teacher = TeacherService.Mapper(teacher, newTeacher);

            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();

            return Ok(new {message = "Cập nhật thông tin giảng viên thành công!"});
        }

        // [GET] /api/teacher/get-by-username
        // Get bằng username không được ghi trên url vì độ bảo mật
        [HttpPost("get-by-username")]
        public async Task<IActionResult> GetByUsername([FromBody] string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("Username is required.");

            var student = await _context.Teachers
                .FirstOrDefaultAsync(s => s.UserName == username);

            if (student == null)
                return NotFound("Teacher not found.");

            return Ok(student);
        }

        // [POST] /api/admin/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return BadRequest("Username is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Password is required.");

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(s => s.UserName == request.Username);

            if (teacher == null)
                return NotFound("Teacher not found.");

            if (teacher.Password != request.Password)
                return BadRequest("Mật khẩu không chính xác.");

            return Ok(new
            {
                teacher.AdminId,
                teacher.FullName,
                teacher.Email,
                teacher.UserName,
                Message = "Đăng nhập thành công!"
            });
        }

        // Xóa mềm giảng viên (set isActive = false)
        [HttpPatch("{id}/soft-delete")]
        public async Task<IActionResult> DeactivateTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound(new { message = "Không tìm thấy giảng viên" });
            }

            if (!teacher.IsActive)
            {
                return BadRequest(new { message = "Giảng viên đã bị xóa trước đó" });
            }

            teacher.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Xóa mềm giảng viên thành công!",
                studentId = teacher.AdminId,
                isActive = teacher.IsActive
            });
        }

        // Khôi phục giảng viên (set isActive = true)
        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> ActivateTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound(new { message = "Không tìm thấy giảng viên" });
            }

            if (teacher.IsActive)
            {
                return BadRequest(new { message = "Giảng viên đang ở trạng thái hoạt động" });
            }

            teacher.IsActive = true;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Khôi phục giảng viên thành công!",
                studentId = teacher.AdminId,
                isActive = teacher.IsActive
            });
        }
    }
}
