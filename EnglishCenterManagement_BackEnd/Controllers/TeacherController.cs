using EnglishCenterManagement_BackEnd.Models;
using EnglishCenterManagement_BackEnd.Service;
using EnglishCenterManagement_BackEnd.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;

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
                return NotFound(ErrorEnums.NOT_FOUND_WITH_MODEL("Giảng viên"));
            }
            return Ok(teacher);
        }

        // [POST] /api/teacher
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Teacher teacher)
        {
            if (teacher == null)
            {
                return BadRequest(ErrorEnums.LACK_OF_FIELD);
            }

            var exist = await _context.Teachers.AnyAsync(t => t.UserName == teacher.UserName);

            if (exist)
            {
                return Conflict(ErrorEnums.USERNAME_EXIST);
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
                    return Conflict(ErrorEnums.SERVER_ERROR);
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
                return NotFound(ErrorEnums.NOT_FOUND_WITH_MODEL("Giảng viên"));
            }

            try
            {
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
                return Ok("Xóa giảng viên thành công.");
            }catch (Exception ex)
            {
                return Conflict(ErrorEnums.SERVER_ERROR);
            }
            
        }

        // [PUT] /api/teacher/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update (int id, [FromBody] Teacher newTeacher)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if(teacher == null)
            {
                return NotFound(ErrorEnums.NOT_FOUND_WITH_MODEL("Giảng viên"));
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
                return BadRequest(ErrorEnums.LACK_OF_FIELD);

            var student = await _context.Teachers
                .FirstOrDefaultAsync(s => s.UserName == username);

            if (student == null)
                return NotFound(ErrorEnums.NOT_FOUND_WITH_MODEL("Giảng viên"));

            return Ok(student);
        }

        // [POST] /api/admin/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(ErrorEnums.LACK_OF_FIELD);

            if (string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(ErrorEnums.LACK_OF_FIELD);

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(s => s.UserName == request.Email);

            if (teacher == null)
                return NotFound(ErrorEnums.NOT_FOUND_WITH_MODEL("Giảng viên"));

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
                return NotFound(ErrorEnums.NOT_FOUND_WITH_MODEL("Giảng viên"));
            }

            if (!teacher.IsActive)
            {
                return BadRequest(ErrorEnums.DATA_REMOVED);
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
                return NotFound(ErrorEnums.NOT_FOUND_WITH_MODEL("Giảng viên"));
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
