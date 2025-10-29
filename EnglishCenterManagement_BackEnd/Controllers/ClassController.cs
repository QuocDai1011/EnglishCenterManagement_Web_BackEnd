using EnglishCenterManagement_BackEnd.Models;
using EnglishCenterManagement_BackEnd.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishCenterManagement_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly EnglishCenterManagementDevContext _context;

        public ClassController(EnglishCenterManagementDevContext context)
        {
            _context = context;
        }

        // [GET] /api/class
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var classes = await _context.Classes.ToListAsync();
            return Ok(classes);
        }

        // [GET] /api/class/get-all-and-courses
        [HttpGet("get-all-and-courses")]
        public async Task<IActionResult> GetAllAndCourse()
        {
            var classes = await _context.Classes
                .Include(c => c.Courses)
                .ToListAsync();
            return Ok(classes);
        }

        // [GET] /api/class/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var _class = await _context.Classes.FindAsync(id);
            return Ok(_class);
        }

        // [POST] /api/class
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Class newClass)
        {
            if (newClass == null) { 
                return BadRequest("Class' data is required!");
            }

            try
            {
                _context.Classes.Add(newClass);

                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new {id = newClass.ClassId}, newClass);
            }
            catch (Exception ex) {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // [DELETE] /api/class/id
        [HttpDelete("{id}")] 
        public async Task<IActionResult> Delete(int id)
        {
            var _class = await _context.Classes.FindAsync(id);
            if(_class == null) return NotFound("Không tìm thấy dữ liệu lớp học!");

            try
            {
                _context.Classes.Remove(_class);
                await _context.SaveChangesAsync();
                return Ok("Xóa lớp học thành công");
            }
            catch (Exception ex) {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        //[PUT] /api/class/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Class updatedClass)
        {
            var _class = await _context.Classes.FindAsync(id);

            if (_class == null) return NotFound("Không tìm thấy dữ liệu lớp học!");

            try
            {
                _class = ClassService.Mapper(_class, updatedClass);
                _context.Classes.Update(updatedClass);
                await _context.SaveChangesAsync();
                return Ok("Cập nhật lớp học thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật khóa học thất bại: {ex.Message}");
            }
        }
    }
}
