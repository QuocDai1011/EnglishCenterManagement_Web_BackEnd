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
        public async Task<IActionResult> GetAll ()
        {
            var teachers = await _context.Teachers.ToListAsync();
            return Ok(teachers);
        }
    }
}
