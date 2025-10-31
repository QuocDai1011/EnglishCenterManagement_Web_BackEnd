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


        //[GET] /api/student
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var students = await _context.Students.ToListAsync();
            return Ok(students);
        }


        //[GET] /api/student/{id}
        [HttpGet("{id}")] 
        public async Task<IActionResult> GetById(int id)
        {
            var students = await _context.Students.FindAsync(id);
            if (students == null) { 
                return NotFound(new { message = "Không tìm thấy sinh viên có ID = " + id });
            }
            return Ok(students);
        }

        // [GET] /api/student/get-by-username
        // Get bằng username không được ghi trên url vì độ bảo mật
        [HttpPost("get-by-username")] 
        public async Task<IActionResult> GetByUsername([FromBody] string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("Username is required.");

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserName == username);

            if (student == null)
                return NotFound("Student not found.");

            return Ok(student);
        }

        // [POST] /api/student
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] Student newStudent)
        {
            if (newStudent == null)
            {
                return BadRequest("Student's data is required");
            }

            // kiểm tra xem username có tồn tại hay chưa
            var exist = await _context.Students.AnyAsync(s => s.UserName == newStudent.UserName);

            if (exist)
            {
                return Conflict(new { message = "Username đã tồn tại, vui lòng chọn tên khác." });
            }

            try
            {
                // thêm vào dbContext
                await _context.Students.AddAsync(newStudent);

                // lưu vào database
                await _context.SaveChangesAsync();

                // Trả về kết quả kèm theo location API của bản ghi mới
                return CreatedAtAction(nameof(GetById), new {id = newStudent.StudentId}, newStudent);
            }
            catch (Exception ex) {
                var inner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // [Delete] /api/student
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent (int id)
        {
            var student = await _context.Students.FindAsync(id);
            if(student == null)
            {
                return NotFound(new { message = "Không tìm thấy sinh viên cần xóa" });
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa học viên thành công!" });
        }

        // PUT: api/student/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] Student updatedStudent)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound(new { message = "Không tìm thấy sinh viên cần cập nhật" });
            }

            // Cập nhật các thuộc tính cần thay đổi
            student.FullName = updatedStudent.FullName;
            student.UserName = updatedStudent.UserName;
            student.Password = updatedStudent.Password;
            student.Email = updatedStudent.Email;
            student.Gender = updatedStudent.Gender;
            student.Address = updatedStudent.Address;
            student.DateOfBirth = updatedStudent.DateOfBirth;
            student.PhoneNumber = updatedStudent.PhoneNumber;
            student.PhoneNumberOfParents = updatedStudent.PhoneNumberOfParents;
            student.UpdateAt = DateOnly.FromDateTime(DateTime.Now);
            student.IsActive = updatedStudent.IsActive;

            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật học viên thành công!" });
        }

    }
}
