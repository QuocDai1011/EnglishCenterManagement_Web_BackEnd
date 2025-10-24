using EnglishCenterManagement_BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishCenterManagement_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly EnglishCenterManagementDevContext _context;

        public StudentController(EnglishCenterManagementDevContext context)
        {
            _context = context;
        }

        //[GET] api/student
        [HttpGet]
        public async Task<IActionResult> GetAllStudent()
        {
            var students = await _context.Students.ToListAsync();
            return Ok(students);
        }
    }
}
