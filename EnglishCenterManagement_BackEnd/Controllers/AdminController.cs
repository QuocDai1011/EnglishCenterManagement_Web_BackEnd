using EnglishCenterManagement_BackEnd.Models;
using EnglishCenterManagement_BackEnd.Service;
using EnglishCenterManagement_BackEnd.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishCenterManagement_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly EnglishCenterManagementDevContext _context;

        public AdminController(EnglishCenterManagementDevContext context)
        {
            _context = context;
        }

        // [GET] /api/admin/get-by-username
        // Get bằng username không được ghi trên url vì độ bảo mật
        [HttpPost("get-by-username")]
        public async Task<IActionResult> GetByUsername([FromBody] string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest(ErrorEnums.LACK_OF_FIELD);

            var student = await _context.Admins
                .FirstOrDefaultAsync(s => s.UserName == username);

            if (student == null)
                return NotFound(ErrorEnums.NOT_FOUND_WITH_MODEL("Admin"));

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

            var admin = await _context.Admins
                .FirstOrDefaultAsync(s => s.Email == request.Email);

            if (admin == null)
                return NotFound(ErrorEnums.NOT_FOUND_WITH_MODEL("Admin"));

            if (admin.Password != request.Password)
                return BadRequest(ErrorEnums.VALIDATION_FAIL);

            return Ok(new
            {
                admin.AdminId,
                admin.FullName,
                admin.Email,
                admin.UserName,
                Message = "Đăng nhập thành công!"
            });
        }

        // [GET] /api/admin
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var admin = await _context.Admins.ToListAsync();
            return Ok(admin);
        }

        // [GET] /api/admin/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound(ErrorEnums.NOT_FOUND_WITH_MODEL("Admin"));
            }

            return Ok(admin);
        }

        // [POST] /api/admin
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Admin newAdmin)
        {
            if (newAdmin == null)
            {
                return BadRequest(ErrorEnums.LACK_OF_FIELD);
            }

            var exist = await _context.Admins.AnyAsync(a => a.UserName == newAdmin.UserName);

            if (exist)
            {
                return Conflict(ErrorEnums.USERNAME_EXIST);
            }
            else
            {
                try
                {
                    _context.Admins.Add(newAdmin);

                    await _context.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetById), new { id = newAdmin.AdminId }, newAdmin);
                }
                catch (Exception ex)
                {
                    return BadRequest(ErrorEnums.SERVER_ERROR);
                }
            }
        }

        // [DELETE] /api/admin/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var admin = await _context.Admins.FindAsync(id);

            if (admin == null)
            {
                return NotFound(ErrorEnums.NOT_FOUND_WITH_MODEL("Admin"));
            }

            try
            {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
                return Ok("Xóa thành công.");
            }
            catch (Exception ex)
            {
                return Conflict(ErrorEnums.SERVER_ERROR);
            }
        }

        // [PUT] /api/admin/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Admin newAdmin)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound(ErrorEnums.NOT_FOUND_WITH_MODEL("Admin"));
            }

            try
            {
                admin = AdminService.Mapper(admin, newAdmin);
                _context.Admins.Update(admin);
                await _context.SaveChangesAsync();
                return Ok(new {message = "Cập nhật dữ liệu thành công."});
            }
            catch (Exception ex) { 
                return Conflict(ErrorEnums.SERVER_ERROR);
            }
        }

        // Xóa mềm admin (set isActive = false)
        [HttpPatch("{id}/soft-delete")]
        public async Task<IActionResult> DeactivateAdmin(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound(ErrorEnums.NOT_FOUND_WITH_MODEL("Admin"));
            }

            if (!admin.IsActive)
            {
                return BadRequest(ErrorEnums.DATA_REMOVED);
            }

            if(admin.IsSuper)
            {
                return Conflict(new { message = "Không thể xóa admin cơ sở." });
            }

            admin.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Xóa mềm giảng viên thành công!",
                studentId = admin.AdminId,
                isActive = admin.IsActive
            });
        }

        // Khôi phục admin (set isActive = true)
        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> ActivateAdmin(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound(ErrorEnums.NOT_FOUND_WITH_MODEL("Admin"));
            }

            if (teacher.IsActive)
            {
                return BadRequest(new { message = "Admin đang ở trạng thái hoạt động" });
            }

            teacher.IsActive = true;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Khôi phục admin thành công!",
                studentId = teacher.AdminId,
                isActive = teacher.IsActive
            });
        }
    }
}
