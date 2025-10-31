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

        // [GET] /api/admin
        //public 
    }
}
