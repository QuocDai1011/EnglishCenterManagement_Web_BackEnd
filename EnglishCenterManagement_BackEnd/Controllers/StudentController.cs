using EnglishCenterManagement_BackEnd.Models;
using EnglishCenterManagement_BackEnd.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishCenterManagement_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var students = await _context.Students
                .Include(s => s.StudentCourses)
                .ToListAsync();
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

            try
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Xóa học viên thành công!" });
            }catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        // PUT: api/student/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Student updatedStudent)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound(new { message = "Không tìm thấy sinh viên cần cập nhật" });
            }

            student = StudentService.Mapper(student, updatedStudent);

            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật học viên thành công!" });
        }

        //[GET] /api/Student/get-classes/{id}
        [HttpGet("get-classes/{id}")]
        public async Task<IActionResult> GetClasses(int id)
        {
            var studentClasses = await _context.Students
                .Include(x => x.Classes)
                .FirstAsync(s => s.StudentId == id);

            if (studentClasses == null) return NotFound("Không tìm thấy dữ liệu!");

            return Ok(studentClasses);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return BadRequest("Username is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Password is required.");

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserName == request.Username);

            if (student == null)
                return NotFound("Student not found.");

            if (student.Password != request.Password)
                return BadRequest("Mật khẩu không chính xác.");

            return Ok(new
            {
                student.StudentId,
                student.FullName,
                student.Email,
                student.UserName,
                Message = "Đăng nhập thành công!"
            });
        }

        // Xóa mềm sinh viên (set isActive = false)
        [HttpPatch("{id}/soft-delete")]
        public async Task<IActionResult> DeactivateStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound(new { message = "Không tìm thấy học viên" });
            }

            if (!student.IsActive)
            {
                return BadRequest(new { message = "Học viên đã bị xóa trước đó" });
            }

            student.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Xóa mềm học viên thành công!",
                studentId = student.StudentId,
                isActive = student.IsActive
            });
        }

        // Khôi phục sinh viên (set isActive = true)
        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> ActivateStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound(new { message = "Không tìm thấy học viên" });
            }

            if (student.IsActive)
            {
                return BadRequest(new { message = "Học viên đang ở trạng thái hoạt động" });
            }

            student.IsActive = true;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Khôi phục học viên thành công!",
                studentId = student.StudentId,
                isActive = student.IsActive
            });
        }
    }
}
