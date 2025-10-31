using EnglishCenterManagement_BackEnd.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace EnglishCenterManagement_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;

        public AuthController(IEmailService emailService, IMemoryCache cache)
        {
            _emailService = emailService;
            _cache = cache;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            _cache.Set(email, otp, TimeSpan.FromMinutes(3)); // TTL = 3 phút

            var body = $"<p>Mã OTP của bạn là: <b>{otp}</b></p><p>OTP có hiệu lực trong 3 phút.</p>";
            await _emailService.SendEmailAsync(email, "Xác thực đăng ký", body);

            return Ok(new { message = "Đã gửi OTP qua Gmail." });
        }

        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp([FromBody] OtpRequest request)
        {
            if (_cache.TryGetValue(request.Email, out string otp) && otp == request.Otp)
            {
                _cache.Remove(request.Email);
                return Ok(new { success = true, message = "Xác thực thành công." });
            }

            return BadRequest(new { success = false, message = "OTP không hợp lệ hoặc đã hết hạn." });
        }
    }

    public class OtpRequest
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}

