using EnglishCenterManagement_BackEnd.Models;
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

        public AdminController (EnglishCenterManagementDevContext context)
        {
            _context = context;
        }

        // [GET] /api/admin/get-by-username
        // Get bằng username không được ghi trên url vì độ bảo mật
        [HttpPost("get-by-username")]
        public async Task<IActionResult> GetByUsername([FromBody] string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("Username is required.");

            var student = await _context.Admins
                .FirstOrDefaultAsync(s => s.UserName == username);

            if (student == null)
                return NotFound("Admin not found.");

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

            var admin = await _context.Admins
                .FirstOrDefaultAsync(s => s.UserName == request.Username);

            if (admin == null)
                return NotFound("Admin not found.");

            if (admin.Password != request.Password)
                return BadRequest("Mật khẩu không chính xác.");

            return Ok(new
            {
                admin.AdminId,
                admin.FullName,
                admin.Email,
                admin.UserName,
                Message = "Đăng nhập thành công!"
            });
        }
    }
}
