using EnglishCenterManagement_BackEnd.Models;
using EnglishCenterManagement_BackEnd.Service;
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
                return NotFound(new { message = "Don't find this admin's information." });
            }

            return Ok(admin);
        }

        // [POST] /api/admin
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Admin newAdmin)
        {
            if (newAdmin == null)
            {
                return BadRequest(new { message = "Admin's data is required." });
            }

            var exist = await _context.Admins.AnyAsync(a => a.UserName == newAdmin.UserName);

            if (exist)
            {
                return Conflict(new { message = "This admin's username exists" });
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
                    return StatusCode(500, $"Internal server error: {ex.Message}");
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
                return NotFound(new { message = "Don't find this admin's infomation." });
            }

            try
            {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
                return Ok("Delete admin's infomation success!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // [PUT] /api/admin/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Admin newAdmin)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound(new { message = "Don't find this admin's information." });
            }

            try
            {
                admin = AdminService.Mapper(admin, newAdmin);
                _context.Admins.Update(admin);
                await _context.SaveChangesAsync();
                return Ok(new {message = "Update the admin's information success!"});
            }
            catch (Exception ex) { 
                return StatusCode(500, "Error: " + ex.Message);
            }
        }
    }
}
