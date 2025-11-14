using EnglishCenterManagement_BackEnd.Models;
using EnglishCenterManagement_BackEnd.Service;
using EnglishCenterManagement_BackEnd.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Claims;

namespace EnglishCenterManagement_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EnglishCenterManagementDevContext _context;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;

        public AuthController(EnglishCenterManagementDevContext context, IEmailService emailService, IMemoryCache cache)
        {
            _context = context;
            _emailService = emailService;
            _cache = cache;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email và Password không được trống.");
            }

            string inputEmail = request.Email.Trim().ToLower();
            string password = request.Password;

            // 1. Phân tích Email để xác định vai trò gợi ý
            var loginInfo = AuthUtils.ParseUsername(inputEmail);
            string role = loginInfo.Role;

            object user = null;
            int userId = 0;

            if (role == "Admin")
            {
                var admin = await _context.Admins
                    .FirstOrDefaultAsync(a => a.Email == inputEmail && a.Password == password && a.IsActive == true);

                if (admin != null)
                {
                    user = admin;
                    userId = admin.AdminId;
                }
            }
            else if (role == "Teacher")
            {
                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => t.Email == inputEmail && t.Password == password && t.IsActive == true);

                if (teacher != null)
                {
                    user = teacher;
                    userId = teacher.AdminId;
                }
            }
            else// Vai trò mặc định
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.Email == inputEmail && s.IsActive == true);

                if (student == null)
                    return Unauthorized("Email không tồn tại hoặc tài khoản bị vô hiệu hóa.");

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, student.Password);
                if (!isPasswordValid)
                    return Unauthorized("Mật khẩu không đúng.");

                user = student;
                userId = student.StudentId;
            }


            if (user == null)
            {
                return Unauthorized("Thông tin đăng nhập không hợp lệ hoặc tài khoản đã bị vô hiệu hóa.");
            }

            // --- 4. TẠO COOKIE XÁC THỰC ---
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Email, inputEmail)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok(new
            {
                UserInfo = new { Id = userId, Email = inputEmail, Role = role },
                Message = $"Đăng nhập vai trò {role} thành công!"
            });
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Xóa Cookie xác thực
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok(new { message = "Đăng xuất thành công." });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.FirstName) ||
               string.IsNullOrWhiteSpace(req.LastName) ||
               string.IsNullOrWhiteSpace(req.Username) ||
               string.IsNullOrWhiteSpace(req.Email) ||
               string.IsNullOrWhiteSpace(req.Address) ||
               string.IsNullOrWhiteSpace(req.PhoneNumber) ||
               string.IsNullOrWhiteSpace(req.PhoneNumberOfParents) ||
               string.IsNullOrWhiteSpace(req.Password) ||
               string.IsNullOrWhiteSpace(req.ConfirmPassword))
            {
                return BadRequest(new { message = "Vui lòng nhập đầy đủ thông tin" });
            }

            if(req.Password != req.ConfirmPassword)
            {
                return BadRequest(new { message = "Password and confirm password do not match" });
            }

            string email = req.Email.Trim();
            bool emailExists = await _context.Students.AnyAsync(s => s.Email == email);
            if (emailExists) return BadRequest(new { message = "Email đã tồn tại trong hệ thống." });

            if (!_cache.TryGetValue(email + "_verified", out bool isOtpVerified) || !isOtpVerified)
                return BadRequest(new { message = "Vui lòng xác thực OTP trước khi đăng ký." });

            // Tạo thông tin người dùng mới
            var fullName = $"{req.LastName.Trim()} {req.FirstName.Trim()}";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(req.Password);

            var student = new Student
            {
                UserName = req.Username,
                Password = hashedPassword,
                FullName = fullName,
                Email = email,
                Address = req.Address,
                Gender = req.Gender,
                DateOfBirth = req.DateOfBirth,
                PhoneNumber = req.PhoneNumber,
                PhoneNumberOfParents = req.PhoneNumberOfParents,
                CreatAt = DateOnly.FromDateTime(DateTime.Now),
                UpdateAt = null,
                IsActive = true,
            };

            try
            {
                await _context.Students.AddAsync(student);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ: " + ex.Message });
            }

            _cache.Remove(email + "_verified");

            return Ok(new { message = "Đăng ký thành công! Bạn có thể đăng nhập ngay" });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email))
            {
                return BadRequest(new { message = "Email trống" });
            }

            string email = req.Email.Trim();
            bool emailExists = await _context.Students.AnyAsync(e => e.Email == email);
            if (!emailExists)
            {
                return BadRequest(new { message = "Email không tồn tại trong CSDL" });
            }
            var otp = new Random().Next(100000, 999999).ToString();
            _cache.Set(email, otp, TimeSpan.FromMinutes(3));

            var body = $"<p>Mã OTP của bạn là: <b>{otp}</b></p><p>OTP có hiệu lực trong 3 phút.</p>";

            try
            {
                await _emailService.SendEmailAsync(email, "Xác thực quên mật khẩu", body);
                return Ok(new { message = "Đã gửi OTP qua Gmail." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Gửi OTP thất bại.", error = ex.Message });
            }
        }


        [HttpGet("currentuser")]
        [Authorize] // Bắt buộc phải đăng nhập (có Cookie) mới truy cập được
        public IActionResult GetCurrentUser()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                // Trường hợp này hiếm xảy ra do [Authorize] đã chặn, nhưng là fallback tốt
                return Unauthorized(new { message = "Chưa đăng nhập hoặc phiên đã hết hạn." });
            }

            // Lấy thông tin từ Claims (đã lưu trong Cookie)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var fullName = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);
            var email = User.FindFirstValue(ClaimTypes.Email);

            return Ok(new
            {
                Id = int.Parse(userId),
                FullName = fullName,
                Email = email,
                Role = role
            });
        }

        [HttpGet("accessdenied")]
        public IActionResult AccessDenied()
        {
            // Endpoint được gọi khi người dùng có Cookie nhưng thiếu quyền (Role)
            return StatusCode(403, new { message = "Bạn không có quyền truy cập vào tài nguyên này." });
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] EmailRequest request)
        {
            var email = request.Email?.Trim();

            if (string.IsNullOrEmpty(email))
                return BadRequest(new { message = "Email không được để trống." });
            if(request.Mode != "resetPassword")
            {
                bool emailExists = await _context.Students.AnyAsync(s => s.Email == email);
                if (emailExists)
                    return BadRequest(new { message = "Email đã tồn tại trong hệ thống." });
            }

            var otp = new Random().Next(100000, 999999).ToString();
            _cache.Set(email, otp, TimeSpan.FromMinutes(3));

            var body = $"<p>Mã OTP của bạn là: <b>{otp}</b></p><p>OTP có hiệu lực trong 3 phút.</p>";

            try
            {
                await _emailService.SendEmailAsync(email, "Xác thực đăng ký", body);
                return Ok(new { message = "Đã gửi OTP qua Gmail." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Gửi OTP thất bại.", error = ex.Message });
            }
        }


        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp([FromBody] OtpRequest request)
        {
            if (_cache.TryGetValue(request.Email, out string otp) && otp == request.Otp)
            {
                _cache.Remove(request.Email);

                _cache.Set(request.Email + "_verified", true, TimeSpan.FromMinutes(10));
                return Ok(new { success = true, message = "Xác thực thành công." });
            }

            return BadRequest(new { success = false, message = "OTP không hợp lệ hoặc đã hết hạn." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassword req)
        {
            string email = req.Email.Trim();
            var user = await _context.Students.FirstOrDefaultAsync(e => e.Email == email);
            if (user == null)
            {
                return BadRequest(new { message = "Email không tồn tại trong CSDL" });
            }

            if (req.NewPassword != req.ConfirmPassword)
            {
                return BadRequest(new { message = "Mật khẩu xác nhận không giống" });
            }

            string hashPassword = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);

            user.Password = hashPassword;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đặt lại mật khẩu thành công" });
        }
    }

    public class OtpRequest
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; } // Dùng Email để đăng nhập
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string FirstName { get; set; }  // Họ
        public string LastName { get; set; }   // Tên
        public string Username { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberOfParents { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class EmailRequest
    {
        public string Email { get; set; }
        public string Mode { get; set; }
    }

    public class ResetPassword
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

}

