using EnglishCenterManagement_BackEnd.Models;
using EnglishCenterManagement_BackEnd.Service;
using Microsoft.AspNetCore.Mvc;

namespace EnglishCenterManagement_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        /// <summary>
        /// Gửi tin nhắn đến trợ lý tư vấn (Gemini AI)
        /// </summary>
        /// <param name="message">Tin nhắn của học viên</param>
        /// <returns>Phản hồi từ AI</returns>
        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserMessage))
                return BadRequest("Tin nhắn không được để trống.");

            try
            {
                var response = await _chatService.GenerateResponseAsync(request.UserMessage);
                return Ok(new
                {
                    success = true,
                    question = request.UserMessage,
                    answer = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi ChatService");
                return StatusCode(500, new
                {
                    success = false,
                    error = "Đã xảy ra lỗi khi xử lý yêu cầu."
                });
            }
        }
    }

}
